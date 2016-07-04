using eCardio.EnterpriseServices.Atlas.Client;
using eCardio.EnterpriseServices.Atlas.Contracts;
using log4net;
using MassTransit;
using Revampness.Services.Contracts;
using Revampness.Services.Paceart.Adapter;
using Revampness.Services.device.Adapter;
using Revampness.Services.device.DataAccess;
using Revampness.Services.device.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TsmRptLibrary
{
    internal class VeritéAutomation
    {
        private const string DbUserString = "EcgFramework.deviceAutomations";

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private int _addHourToDateForMinMaxComplete = 2;
        private Guid _defaultPatientGuid = Guid.Empty;
        private string _domain = string.Empty;
        private string _domainPassword = string.Empty;
        private string _domainUsername = string.Empty;
        private string _ecgFilePath = string.Empty;
        private bool _enablePaceartEventQueue = false;
        private bool _enableViaAutoRequest = false;
        private int _maxEaeErrorAttempts = 10;
        private DateTime _minimumDate = DateTime.MinValue;
        private bool _orderEventAutomationEntries = false;
        private string _otaArchiveRoot = string.Empty;
        private string _otaQueue = string.Empty;
        private string _paceartEventQueue = string.Empty;
        private string _queue = string.Empty;
        private bool _sendEmailEnableSsl = false;
        private string _sendEmailFrom = string.Empty;
        private bool _sendEmailOnError = false;
        private string _sendEmailPassword = string.Empty;
        private int _sendEmailPort = 0;
        private string _sendEmailServer = string.Empty;
        private string _sendEmailSubject = "ECG Framework Error";
        private List<string> _sendEmailTo = new List<string>();
        private string _sendEmailUsername = string.Empty;
        private string _deviceAutomationItemQueue = string.Empty;

        public VeritéAutomation()
        {
            this.LoadServiceData();
        }

        /// <summary>
        ///     Process the EventAutomationEntry table.
        /// </summary>
        /// <param name="maxDate">
        ///     The maximum date to pull from the queue.  When running in production, this will be DateTime.Now.
        ///     This is a parameter for scripting and testing.
        /// </param>
        /// <param name="runDate">
        ///     The date which this is run. When running in produciton, this will be DateTime.Now.
        ///     This is a parameter for scripting and testing.
        /// </param>
        public void ProcessAutomationEntries(DateTime maxDate, DateTime runDate)
        {
            try
            {
                var repository = new deviceRepository();
                //var errantEaes = new List<EventAutomationEntry>();
                //var eventAutomationEntries = resp.GetUnrequestedEventAutomationEntries(maxDate);

                //we need to convert the result to an IList since we save new records for min/max to the db. We might want to rethink
                //how we are saving these records and only call the .SaveChanges() after this forloop so that we do not need to
                //load everything at once into a list.
                //2014-07-14 - if we want the newest entries to be requested first, set orderEventAutomationEntries to true in the config
                var eventAutomationEntries = this._orderEventAutomationEntries ? repository.GetUnrequestedEventAutomationEntriesOrdered(maxDate) : repository.GetUnrequestedEventAutomationEntries(maxDate);
                var eventAutomationEntryIds = eventAutomationEntries.Select(x => x.Id).ToList();

                foreach (var eventAutomationEntryId in eventAutomationEntryIds)
                {
                    this.LogMe("Debug", "eaeId: " + eventAutomationEntryId, null);
                }

                if (eventAutomationEntryIds.Any())
                {
                    Parallel.ForEach(eventAutomationEntryIds, id =>
                                                              {
                                                                  var threadId = Thread.CurrentThread.ManagedThreadId;
                                                                  this.LogMe("Information", "Begin processing eadId: " + id, null, threadId);
                                                                  var resp = new deviceRepository();
                                                                  var eae = resp.GetEventAutomationEntryByIdWithDevice(id);
                                                                  EventAutomationEntry errEae = null;

                                                                  var drt = EcgServiceEnums.GetDataRequestType(eae.DataRequestTypeId);

                                                                  switch (drt)
                                                                  {
                                                                      case EcgServiceEnums.DataRequestType.MinimumHr:
                                                                          //2:
                                                                          //Min(2)
                                                                          errEae = this.ProcessAutomationEntryMinMax(resp, eae, true, runDate, threadId);
                                                                          break;

                                                                      case EcgServiceEnums.DataRequestType.MaximumHr:
                                                                          //3:
                                                                          //Max(3)
                                                                          errEae = this.ProcessAutomationEntryMinMax(resp, eae, false, runDate, threadId);
                                                                          break;

                                                                      case EcgServiceEnums.DataRequestType.ProactiveMinimumHr: //4:
                                                                      case EcgServiceEnums.DataRequestType.ProactiveMaximumHr: //5:
                                                                                                                               //Proactive Min(4)/Max(5)
                                                                          this.LogMe("Warning", "DataRequestType: " + drt.ToString() + " (" + eae.DataRequestType.RequestDescription + " -- placeholder EAE) should NEVER need automation.", null);
                                                                          break;

                                                                      case EcgServiceEnums.DataRequestType.Timed: // 6:
                                                                                                                  //Timed
                                                                          errEae = this.ProcessAutomationEntryTimed(resp, eae, threadId);
                                                                          break;

                                                                      default:
                                                                          this.LogMe("Warning", "DataRequestType: " + drt.ToString() + " (" + eae.DataRequestType.RequestDescription + ") is not supported for automation processing.", null);
                                                                          break;
                                                                  }
                                                              });
                }
                else
                {
                    this.LogMe("Information", "No unprocessed event automations found using maxDate: " + maxDate, null);
                }
                //get list of all timed & min/max events that haven't been sent yet
                //for each event
                // - if timed, query
            }
            catch (AggregateException ax)
            {
                foreach (var x in ax.InnerExceptions)
                {
                    this.LogMe("Error", "ProcessAutomationEntries ERROR: " + x.Message, x);
                }
            }
            catch (Exception x)
            {
                this.LogMe("Error", "ProcessAutomationEntries ERROR: " + x.Message, x);
            }
        }

        public List<AutomationItem> ProcessAutomationEntriesFromQueue(int maxRetries, int sqlCommandTimeout)
        {
            var rtn = new List<AutomationItem>();

            if (this._deviceAutomationItemQueue == string.Empty)
            {
                throw new DataException("MSMQ queue name cannot be blank.");
            }

            this.LogMe("Information", "Processings messages in queue: " + this._deviceAutomationItemQueue, null);
            var runDate = DateTime.Now;
            var queue = new MessageQueue(this._deviceAutomationItemQueue);
            var ser = new DataContractJsonSerializer(typeof(AutomationItem));
            var moreMessages = true;
            //Message msgPeek = null;
            Message msg = null;
            var ts = new TimeSpan(0);

            while (moreMessages)
            {
                msg = null;

                try
                {
                    msg = queue.Receive(ts);
                }
                catch (Exception)
                {
                    this.LogMe("Information", "No more messages found in queue (timeout).", null);
                    moreMessages = false;
                    msg = null;
                    break;
                }

                if (msg == null)
                {
                    this.LogMe("Information", "No more messages found in queue (null message).", null);
                    //if the message is null, don't check for more messages and end the run
                    moreMessages = false;
                }
                else
                {
                    this.LogMe("Debug", "Found message: " + msg.Id, null);
                    var ai = (AutomationItem)ser.ReadObject(msg.BodyStream);
                    this.LogMe("Information", "Processing message: " + ai.EventAutomationEntryId, null);
                    if (ai.LastAttemptDate > runDate)
                    {
                        //if our lastAttemptDate is newer than when this run started, assume that we have attempted
                        //to process all available messages and end the run
                        moreMessages = false;

                        this.SaveMessageToQueue(ai);

                        this.LogMe("Information", "Message " + ai.EventAutomationEntryId + " (Retries: " + ai.RetryCounter + ") last processed " + ai.LastAttemptDate + " is greater than current run (" + runDate + "). Ending run.", null);
                    }
                    else
                    {
                        //good message
                        var aiRtn = this.ProcessAutomationItem(ai, sqlCommandTimeout);

                        if (aiRtn == null)
                        {
                            this.LogMe("Information", "Message successfully processed and removed.", null);
                        }
                        else
                        {
                            if (aiRtn.RetryCounter > maxRetries)
                            {
                                rtn.Add(aiRtn);
                            }
                            else
                            {
                                this.LogMe("Warning", "Issue with processing message, returning to queue.", null);
                                if (aiRtn.ProcessMessage != string.Empty)
                                {
                                    this.LogMe("Warning", " - ProcessMessage: " + aiRtn.ProcessMessage, null);
                                    aiRtn.ProcessMessage = string.Empty;
                                }
                                if (aiRtn.ProcessStackTrace != string.Empty)
                                {
                                    this.LogMe("Warning", " - StackTrace: " + aiRtn.ProcessStackTrace, null);
                                    aiRtn.ProcessStackTrace = string.Empty;
                                }
                                this.SaveMessageToQueue(aiRtn);
                            }
                        }
                    }
                }
            } //while()

            this.LogMe("Information", "Done processing queue.", null);
            return rtn;
        }

        public void ProcessAutomationEntriesSerial(DateTime maxDate, DateTime runDate)
        {
            try
            {
                var resp = new deviceRepository();
                //var errantEaes = new List<EventAutomationEntry>();
                //var eventAutomationEntries = resp.GetUnrequestedEventAutomationEntries(maxDate);

                //we need to convert the result to an IList since we save new records for min/max to the db. We might want to rethink
                //how we are saving these records and only call the .SaveChanges() after this forloop so that we do not need to
                //load everything at once into a list.
                //2014-07-14 - if we want the newest entries to be requested first, set orderEventAutomationEntries to true in the config
                IList<EventAutomationEntry> eventAutomationEntries = this._orderEventAutomationEntries ? resp.GetUnrequestedEventAutomationEntriesOrdered(maxDate).ToList() : resp.GetUnrequestedEventAutomationEntries(maxDate).ToList();

                if (eventAutomationEntries != null && eventAutomationEntries.Any())
                {
                    foreach (var eae in eventAutomationEntries)
                    {
                        this.ProcessSingleAutomationEntry(eae, runDate, resp);
                    }
                }
                else
                {
                    this.LogMe("Information", "No unprocessed event automations found using maxDate: " + maxDate, null);
                }
                //get list of all timed & min/max events that haven't been sent yet
                //for each event
                // - if timed, query
            }
            catch (Exception x)
            {
                this.LogMe("Error", "ProcessAutomationEntries ERROR: " + x.Message, x);

                //rethrow exeception so emailing error gets caught
                throw;
            }
        }

        public void ProcessAutomationEntriesToQueue(DateTime maxDate, DateTime runDate)
        {
            try
            {
                var resp = new deviceRepository();
                //var errantEaes = new List<EventAutomationEntry>();
                //var eventAutomationEntries = resp.GetUnrequestedEventAutomationEntries(maxDate);

                //we need to convert the result to an IList since we save new records for min/max to the db. We might want to rethink
                //how we are saving these records and only call the .SaveChanges() after this forloop so that we do not need to
                //load everything at once into a list.
                //2014-07-14 - if we want the newest entries to be requested first, set orderEventAutomationEntries to true in the config
                IList<EventAutomationEntry> eventAutomationEntries = this._orderEventAutomationEntries ? resp.GetUnrequestedEventAutomationEntriesOrdered(maxDate).ToList() : resp.GetUnrequestedEventAutomationEntries(maxDate).ToList();

                if (eventAutomationEntries != null && eventAutomationEntries.Any())
                {
                    foreach (var eae in eventAutomationEntries)
                    {
                        var eaeItem = new Verité.Model.AutomationItem();
                        eaeItem.EventAutomationEntryId = eae.Id;
                        eaeItem.RunDate = runDate;
                        eaeItem.OriginalQueueDate = DateTime.Now;
                        eaeItem.RetryCounter = 0;
                        eaeItem.LastAttemptDate = DateTime.MinValue;
                        eaeItem.ProcessMessage = string.Empty;
                        eaeItem.ProcessStackTrace = string.Empty;

                        SaveMessageToQueue(eaeItem);
                    }
                }
                else
                {
                    this.LogMe("Information", "No unprocessed event automations found using maxDate: " + maxDate, null);
                }
                //get list of all timed & min/max events that haven't been sent yet
                //for each event
                // - if timed, query
            }
            catch (Exception x)
            {
                this.LogMe("Error", "ProcessAutomationEntries ERROR: " + x.Message, x);

                //rethrow exeception so emailing error gets caught
                throw;
            }
        }

        /// <summary>
        ///     Post-process startup automation logic (update patientId setting, event automations & unassigned events)
        /// </summary>
        /// <param name="sa">
        ///     StartupAutomation message which contains the Tze file, original queue date, retry attemps and last attempted
        ///     process date
        /// </param>
        /// <returns>
        ///     null on a successful process or a populated StartupAutomation message with retryCounter incremented and
        ///     lastAttemptedProcess date populated.
        /// </returns>
        public StartupAutomation ProcessStartupAutomationQueueMessage(StartupAutomation sa)
        {
            var saRtn = new StartupAutomation();

            try
            {
                this.LogMe("Debug", "ENTER: EcgService.Verité.Adapter.VeritéAutomation.ProcessStartupAutomationQueueMessage", null);

                this.LogMe("Information", "Automation Attempt: " + sa.Tze.Filename + " | Tag: " + sa.Tze.Tag + " | RecordDate: " + sa.Tze.RecordDate + " | QueueDate: " + sa.OriginalQueueDate, null);
                this.LogMe("Information", " - Retries: " + sa.RetryCounter + " | Last Attempt: " + sa.LastAttemptDate, null);

                var patientGuid = Guid.Empty;
                PaceartAdapter.PatientEnrollmentDates ped = null;
                var pa = new PaceartAdapter.PaceartAdapter();

                //2014-07-15 - mjandes
                //We cannot use tze.PatientId as changes to this can kill devices!

                //if (sa.Tze.PatientId != string.Empty) {
                //    var tmpG = Guid.Empty;
                //    if (Guid.TryParse(sa.Tze.PatientId, out tmpG))
                //        patientGuid = tmpG;
                //    else {
                //        patientGuid = Guid.Empty;
                //    }
                //}

                //if (patientGuid == Guid.Empty) {
                //    LogMe("Debug", "TZE PatientId detected as blank: " + sa.Tze.PatientId, null);
                //    //this will happen if patientid is blank or we couldn't parse it

                //    LogMe("Debug", "Getting Paceart enrollment information...", null);

                //    //2014-07-14 - we can not use a dated get here as the implantDate will ALWAYS be > than the
                //    //first recordDate since it is not set until at least the first transmission is sent which would
                //    //make it ALWAYS > recordDate.  Instead, we are going to use the same logic that the main framework
                //    //uses for finding the most recent patient. The only thing we are adding is to make sure implantDate
                //    //is not null either.
                //    //ped = pa.GetdevicePaceartDeviceFromPaceartForDsrScripting(sa.Tze.SerialNumber,
                //    //    GetDateToUseForPatientDetermination(sa.Tze.RecordDate, sa.OriginalQueueDate));
                //    ped = pa.GetdevicePaceartMostRecentEnrollmentFromDeviceSerialNoExplant(sa.Tze.SerialNumber);

                //    if (ped != null) {
                //        //if (patientGuid != Guid.Empty && patientGuid != _defaultPatientGuid) {
                //        if (ped.PatientGuid != Guid.Empty && ped.PatientGuid != _defaultPatientGuid) {
                //            patientGuid = ped.PatientGuid;
                //            UpdateDeviceWithPatientId(sa.Tze.SerialNumber, patientGuid);
                //        }
                //        else {
                //            LogMe("Information",
                //                "Could not find valid patient data in Paceart for serial: " + sa.Tze.SerialNumber +
                //                " patGuid(" + ped.PatientGuid + ")", null);
                //        }
                //    }
                //    else {
                //        LogMe("Information",
                //            "Could not find valid enrollment data in Paceart for serial: " + sa.Tze.SerialNumber, null);
                //    }
                //}

                //if (patientGuid == Guid.Empty) {
                //    throw new InvalidDataException("Could not find a valid patient using file: " + sa.Tze.Filename +
                //                                   " - Serial: " + sa.Tze.SerialNumber + " | RecordDate: " +
                //                                   sa.Tze.RecordDate);
                //}

                //if (patientGuid == _defaultPatientGuid) {
                //    throw new InvalidDataException("Default patient found using file: " + sa.Tze.Filename +
                //                                   " - Serial: " + sa.Tze.SerialNumber + " | RecordDate: " +
                //                                   sa.Tze.RecordDate);
                //}

                ////We have a valid patient, continue with automation
                //if (ped == null) {
                //    //we have not tried to call this, try to get this data
                //    LogMe("Debug", "Getting Paceart enrollment information...", null);
                //    ped = pa.GetdevicePaceartMostRecentEnrollmentFromDeviceSerialPatientGuid(sa.Tze.SerialNumber,
                //        patientGuid);
                //}

                ped = pa.GetdevicePaceartMostRecentEnrollmentFromDeviceSerialNoExplant(sa.Tze.SerialNumber);

                if (ped == null)
                {
                    throw new Exception("Couldn't find valid enrollment data using serial: " + sa.Tze.SerialNumber + " | patientGuid: " + patientGuid);
                }

                if (sa.Tze.Tag == 1)
                {
                    this.LogMe("Debug", "Found Recording Started Tag: Pushing message for Patient Guid: " + patientGuid + " and Device Serial Number: " + sa.Tze.SerialNumber, null);
                    this.PushRecordingStartedMessageToQueue(patientGuid, sa.Tze.SerialNumber);
                }

                //  a - Get Atlas settings
                this.LogMe("Debug", "Getting atlas items for patientGuid: " + ped.PatientGuid, null);
                var aci = this.GetAtlasItems(ped.PatientGuid);

                this.LogMe("Debug", "Processing event automations...", null);
                this.ProcessEventAutomations(sa.Tze.SerialNumber, ped, aci);

                this.LogMe("Debug", "Checking for existing default patients...", null);
                this.CheckAndUpdateExistingDefaultRecords(sa.Tze.SerialNumber, ped);

                //2015-04-07 - mjandes
                var viaa = new ViaAutomation();
                viaa.CreateViaRequests(sa.Tze, aci, ped.PatientGuid);

                saRtn = null;
            }
            catch (Exception x)
            {
                this.LogMe("FatalError", "ERROR: " + x.Message, x);
                saRtn.Tze = sa.Tze;
                saRtn.OriginalQueueDate = sa.OriginalQueueDate;
                saRtn.LastAttemptDate = DateTime.Now;
                saRtn.RetryCounter = sa.RetryCounter + 1;
            }

            this.LogMe("Debug", "EXIT: EcgService.Verité.Adapter.VeritéAutomation.ProcessStartupAutomationQueueMessage", null);
            return saRtn;
        }

        /// <summary>
        ///     Save TZE message to MSMQ for startup automation post-processing.
        ///     The message will complete post-processing ONLY when a valid enrollment is found and has valid dates populated on
        ///     the enrollment.
        /// </summary>
        /// <param name="tze">TzEvent file parsed to the class</param>
        /// <param name="originalQueueDate">Initial date message was added to the queue</param>
        /// <param name="retryCounter">Number or times processing of this message has been attempted</param>
        /// <param name="lastAttemptDate">The last time an attempt to process this message was made.</param>
        public void SaveMessageToQueue(EventLog tze, DateTime originalQueueDate, int retryCounter, DateTime lastAttemptDate)
        {
            try
            {
                if (tze.Tag != 1)
                {
                    throw new DataException("Only TZE Tag 1 (Recording Started) can be added to device Automation Queue");
                }

                this.LogMe("Debug", "Building StartupAutomation class...", null);
                var sa = new StartupAutomation();
                sa.OriginalQueueDate = originalQueueDate;
                sa.RetryCounter = retryCounter;
                sa.LastAttemptDate = lastAttemptDate;
                sa.Tze = tze;

                this.SaveMessageToQueue(sa);
            }
            catch (Exception x)
            {
                this.LogMe("Error", "MSMQ Error: " + x.Message, x);
                //LogMe("Information", " * Continuing on -- serial (" + tze.Filename + ") will not be processed.", null);
            }
        }

        public void StartupQueueProcessor()
        {
            //message.peek
            //if (date> initial run date or no more messages)
            //      end
            // else
            //      get message from queue
            //      get enrollment
            //      if valid enrollment
            //          process message
            //      else
            //          if message > error threshold
            //              write to file system
            //              email
            //          else
            //              incremente error items
            //              write back to queue
            //          end if
            //      end if
            //end if
            if (this._queue == string.Empty)
            {
                throw new DataException("MSMQ queue name cannot be blank.");
            }

            this.LogMe("Information", "Processings messages in queue: " + this._queue, null);
            var runDate = DateTime.Now;
            var queue = new MessageQueue(this._queue);
            var ser = new DataContractJsonSerializer(typeof(StartupAutomation));
            var moreMessages = true;
            Message msgPeek = null;
            var ts = new TimeSpan(0);

            while (moreMessages)
            {
                msgPeek = null;

                try
                {
                    msgPeek = queue.Peek(ts);
                }
                catch (Exception)
                {
                    this.LogMe("Information", "No more messages found in queue (timeout).", null);
                    moreMessages = false;
                    msgPeek = null;
                    break;
                }

                if (msgPeek == null)
                {
                    this.LogMe("Information", "No more messages found in queue (null message).", null);
                    //if the message is null, don't check for more messages and end the run
                    moreMessages = false;
                }
                else
                {
                    this.LogMe("Debug", "Found message: " + msgPeek.Id, null);
                    var sa = (StartupAutomation)ser.ReadObject(msgPeek.BodyStream);
                    this.LogMe("Information", "Processing message: " + sa.Tze.Filename, null);
                    if (sa.LastAttemptDate > runDate)
                    {
                        //if our lastAttemptDate is newer than when this run started, assume that we have attempted
                        //to process all available messages and end the run
                        moreMessages = false;
                        this.LogMe("Information", "Message " + sa.Tze.Filename + " (Retries: " + sa.RetryCounter + ") last processed " + sa.LastAttemptDate + " is greater than current run (" + runDate + "). Ending run.", null);
                    }
                    else
                    {
                        //good message
                        var saRtn = this.ProcessStartupAutomationQueueMessage(sa);

                        this.LogMe("Debug", "Removing message from queue...", null);

                        //do a formal queue.receive to remove it from the queue (peek up above does not remove it)
                        var message = queue.Receive(ts);

                        if (saRtn == null)
                        {
                            this.LogMe("Information", "Message successfully processed and removed.", null);
                        }
                        else
                        {
                            //TODO: Add error limit processing here
                            this.LogMe("Warning", "Issue with processing message, returning to queue.", null);
                            this.SaveMessageToQueue(saRtn);
                        }
                    }
                }
            } //while()

            this.LogMe("Information", "Done processing queue.", null);
        }

        private AutomationItem ProcessAutomationItem(AutomationItem ai, int sqlCommandTimeout)
        {
            var aiRtn = new AutomationItem();

            try
            {
                this.LogMe("Debug", "ENTER: EcgService.Verité.Adapter.VeritéAutomation.ProcessAutomationItem", null);

                this.LogMe("Information", "Automation Attempt: " + ai.EventAutomationEntryId + " | RunDate: " + ai.RunDate + " | QueueDate: " + ai.OriginalQueueDate, null);
                this.LogMe("Information", " - Retries: " + ai.RetryCounter + " | Last Attempt: " + ai.LastAttemptDate, null);

                var resp = new deviceRepository(sqlCommandTimeout);

                var eae = resp.GetEventAutomationEntryById(ai.EventAutomationEntryId);

                this.ProcessSingleAutomationEntry(eae, ai.RunDate, resp);

                aiRtn = null;
            }
            catch (Exception x)
            {
                this.LogMe("FatalError", "ERROR: " + x.Message, x);
                aiRtn.EventAutomationEntryId = ai.EventAutomationEntryId;
                aiRtn.RunDate = ai.RunDate;
                aiRtn.OriginalQueueDate = ai.OriginalQueueDate;
                aiRtn.LastAttemptDate = DateTime.Now;
                aiRtn.RetryCounter = ai.RetryCounter + 1;
                aiRtn.ProcessMessage = x.Message;
                if (x.InnerException != null)
                {
                    aiRtn.ProcessStackTrace = x.InnerException.StackTrace;
                }
            }

            this.LogMe("Debug", "EXIT: EcgService.Verité.Adapter.VeritéAutomation.ProcessAutomationItem", null);
            return aiRtn;
        }

        private void ProcessSingleAutomationEntry(EventAutomationEntry eae, DateTime runDate, deviceRepository resp)
        {
            EventAutomationEntry errEae = null;

            var drt = EcgServiceEnums.GetDataRequestType(eae.DataRequestTypeId);

            switch (drt)
            {
                case EcgServiceEnums.DataRequestType.MinimumHr:
                    //2:
                    //Min(2)
                    errEae = this.ProcessAutomationEntryMinMax(resp, eae, true, runDate, 0);
                    break;

                case EcgServiceEnums.DataRequestType.MaximumHr:
                    //3:
                    //Max(3)
                    errEae = this.ProcessAutomationEntryMinMax(resp, eae, false, runDate, 0);
                    break;

                case EcgServiceEnums.DataRequestType.ProactiveMinimumHr: //4:
                case EcgServiceEnums.DataRequestType.ProactiveMaximumHr: //5:
                    //Proactive Min(4)/Max(5)
                    this.LogMe("Warning", "DataRequestType: " + drt.ToString() + " (" + eae.DataRequestType.RequestDescription + " -- placeholder EAE) should NEVER need automation.", null);
                    break;

                case EcgServiceEnums.DataRequestType.Timed: // 6:
                    //Timed
                    errEae = this.ProcessAutomationEntryTimed(resp, eae, 0);
                    break;

                default:
                    this.LogMe("Warning", "DataRequestType: " + drt.ToString() + " (" + eae.DataRequestType.RequestDescription + ") is not supported for automation processing.", null);
                    break;
            }
        }

        /// <summary>
        ///     Since we have been notified that a new recording has been started, we will push to our queue a message with the
        ///     patient guid and the serialnumber of the device.
        /// </summary>
        /// <param name="PatientGuid">The Guid of the patient who is starting a new recording</param>
        /// <param name="SerialNumber">Serial Number associated with the PatientGuid</param>
        private void PushRecordingStartedMessageToQueue(Guid PatientGuid, string SerialNumber)
        {
            try
            {
                string subscriptionServiceAddress = ConfigurationManager.AppSettings["ServiceBus.SubscriptionServiceAddress"];
                string receiveFromAddress = ConfigurationManager.AppSettings["ServiceBus.ReceiveFromAddress"];
                string enableEsb = ConfigurationManager.AppSettings["ServiceBus.Enabled"];

                var recordingstartedmessage = new ReceivedRecordingStartedMessage();
                recordingstartedmessage.PatientGuid = PatientGuid;
                recordingstartedmessage.SerialNumber = SerialNumber;

                if (enableEsb == null || enableEsb.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                {
                    try
                    {
                        this.LogMe("Debug", "The Service Bus requires an active consumer. Service Bus starting with Subscription Service Address: " + subscriptionServiceAddress + " and Receiving From: " + receiveFromAddress, null);
                        Bus.Start(subscriptionServiceAddress, receiveFromAddress);

                        this.LogMe("Debug", "Publishing message ReceivedRecordingStartedMessage for patient: " + recordingstartedmessage.PatientGuid + ", and device serialnumber: " + recordingstartedmessage.SerialNumber, null);
                        Bus.Publish(recordingstartedmessage);
                    }
                    catch (Exception e)
                    {
                        this.LogMe("Error", "Unable to start consumer, therefore recording started message will not be published: " + e, null);
                    }
                }
            }
            catch (Exception e)
            {
                this.LogMe("Error", "Unable to get Service Bus Config Values, therefore recording started message will not be published: " + e, null);
            }
        }

        private void SaveMessageToQueue(StartupAutomation sa)
        {
            this.LogMe("Debug", "Createing MSMQ message...", null);
            var ser = new DataContractJsonSerializer(typeof(StartupAutomation));

            var stream1 = new MemoryStream();
            ser.WriteObject(stream1, sa);
            stream1.Position = 0;

            var msgTx = new MessageQueueTransaction();

            var message = new Message();
            message.Label = sa.Tze.Filename;
            message.BodyStream = stream1;

            var msgQ = new MessageQueue(this._queue);

            this.LogMe("Debug", "Writing MSMQ message to queue: " + this._queue, null);
            msgTx.Begin();
            msgQ.Send(message, msgTx);
            msgTx.Commit();

            this.LogMe("Information", "+ Message added to queue: " + sa.Tze.Filename + " | " + sa.Tze.Tag, null);
        }

        private void SaveMessageToQueue(AutomationItem ai)
        {
            this.LogMe("Debug", "Createing MSMQ message...", null);
            var ser = new DataContractJsonSerializer(typeof(AutomationItem));

            var stream1 = new MemoryStream();
            ser.WriteObject(stream1, ai);
            stream1.Position = 0;

            var msgTx = new MessageQueueTransaction();

            var message = new Message();
            message.Label = ai.EventAutomationEntryId.ToString() + "-" + ai.RunDate.ToString();
            message.BodyStream = stream1;

            var msgQ = new MessageQueue(this._deviceAutomationItemQueue);

            this.LogMe("Debug", "Writing MSMQ message to queue: " + this._deviceAutomationItemQueue, null);
            msgTx.Begin();
            msgQ.Send(message, msgTx);
            msgTx.Commit();

            this.LogMe("Information", "+ Message added to queue: " + ai.EventAutomationEntryId, null);
        }

        #region Private Methods

        /// <summary>
        ///     Processes Minimum EventAutomationEntries.  Entries processed after AutomationDate + 1 Day + PadHours (default: 2)
        ///     will be marked as errored or requested.
        /// </summary>
        /// <param name="resp">device Repository</param>
        /// <param name="eae">EventAutomationEntry to process</param>
        /// <param name="isMin">true if this is for MinimumHR, false if for MaximumHR</param>
        /// <param name="runDate">
        ///     This should be DateTime.Now unless we are doing scripting and need it for something else (usually testing). This
        ///     date is what is used
        ///     for the comparison to see if we need to complete the request.
        /// </param>
        /// <param name="threadId"></param>
        /// <returns>If eae is errant, returns the entry for post processing emails</returns>
        private EventAutomationEntry ProcessAutomationEntryMinMax(deviceRepository resp, EventAutomationEntry eae, bool isMin, DateTime runDate, int threadId)
        {
            EventAutomationEntry errantEae = null;
            var device = eae.EventAutomation.Device;

            var endDate = eae.AutomationDate.AddDays(1);
            var endDateComplete = endDate.AddHours(this._addHourToDateForMinMaxComplete);
            var isError = false;
            var fileExists = false;
            DataRequest dr = null;
            TzIntervalReportEntry ire = null;

            var drt = EcgServiceEnums.DataRequestType.MaximumHr;
            var drtPro = EcgServiceEnums.DataRequestType.ProactiveMaximumHr;

            bool isRunDateComplete = runDate > endDateComplete;

            if (isMin)
            {
                this.LogMe("Information", "Getting TZR record TAG 19 for SCP file determination (Min EaeId: " + eae.Id + ")...", null, threadId);
                drt = EcgServiceEnums.DataRequestType.MinimumHr;
                drtPro = EcgServiceEnums.DataRequestType.ProactiveMinimumHr;
                ire = resp.GetMinIntervalEntry(device.SerialNumber, eae.AutomationDate, endDate);
            }
            else
            {
                this.LogMe("Information", "Getting TZR record TAG 19 for SCP file determination (Max EaeId: " + eae.Id + ")...", null, threadId);
                ire = resp.GetMaxIntervalEntry(device.SerialNumber, eae.AutomationDate, endDate);
            }

            if (ire == null)
            {
                this.LogMe("Warning", "Could not find valid HR TZR record for automation type " + drt.ToString() + " (" + device.SerialNumber + " - " + eae.AutomationDate + ")", null, threadId);

                eae.RequestAttempts++;
                isError = true;
            }
            else
            {
                this.LogMe("Information", "Valid HR TZR record for automation type " + drt.ToString() + " (" + device.SerialNumber + " - " + eae.AutomationDate + ")", null, threadId);

                this.LogMe("Information", "Checking to see if we have already requested SCP file: " + ire.SequenceNumber + "...", null, threadId);

                //Check to see if we are already requested this sequencenumber for this serial/date
                dr = resp.GetDataRequestByPatientScp(eae.EventAutomation.PatientGuid, device.Id, ire.SequenceNumber);
                if (dr == null)
                {
                    //We need to create a new proactive eae & data request
                    this.LogMe("Information", "Request for SCP file: " + ire.SequenceNumber + " not found, creating new request...", null, threadId);

                    if (isRunDateComplete)
                    {
                        //if we are creating a request after the date buffer we assume that the request
                        //is the absolute min/max and we are going to use it for the reports.  we then
                        //do not need to create a proactive event and can set the request to the actual min/max
                        fileExists = this.CheckIfScpFileExists(device.SerialNumber, ire.SequenceNumber);
                        dr = this.CreateDataRequest(resp, device, eae, ire, fileExists, threadId);
                    }
                    else
                    {
                        var proEae = this.CreateNewEventAutomationEntry(resp, eae.EventAutomation, eae, drtPro, threadId);
                        fileExists = this.CheckIfScpFileExists(device.SerialNumber, ire.SequenceNumber);
                        dr = this.CreateDataRequest(resp, device, proEae, ire, fileExists, threadId);
                    }
                }
            }

            //Check to see if we are running after the last allotted time to run this request.
            if (isRunDateComplete)
            {
                if (dr == null || isError == true)
                {
                    this.LogMe("Information", "Setting EAE: " + eae.Id + " to IsError.", null, threadId);
                    eae.IsError = true;
                    errantEae = eae;
                }
                else
                {
                    this.LogMe("Information", "Setting EAE: " + eae.Id + " to IsRequested.", null, threadId);
                    eae.DataRequestId = dr.Id;
                    eae.RequestDate = DateTime.Now;
                    eae.IsRequested = true;

                    //double check for file (in case we didnt check ^^^^)
                    if (!fileExists)
                    {
                        fileExists = this.CheckIfScpFileExists(device.SerialNumber, ire.SequenceNumber);
                    }
                    if (fileExists)
                    {
                        //the file is here, lets process it now
                        var eventGuid = this.SubmitEventToPaceart(device, ire, eae.EventAutomation.PatientGuid, resp, drt, eae, threadId);

                        if (eventGuid != Guid.Empty)
                        {
                            this.LogMe("Information", "Event stored in Paceart (Patient: " + eae.EventAutomation.PatientGuid + "EvGuid: " + eventGuid + "), completing data request... ", null, threadId);
                            dr.EventGuid = eventGuid;
                            dr.IsCompleted = true;
                            dr.CompletedAt = DateTime.Now;
                            dr.CompletedBy = DbUserString;
                            resp.UpdateDataRequest(dr, false); //we'll save changes below.
                        }
                        else
                        {
                            this.LogMe("Warning", "EventGuid is empty when submitting " + drt.ToString() + " - dataRequest.Id: " + dr.Id + " to Paceart.", null, threadId);
                        }
                    }
                    else
                    {
                        //we are still waiting for the selected request to come in, set the request
                        //to a non pro-active type so that it will get processed when it comes in
                        var drtDescription = "[DataRequest.DataRequestType IS NULL]";
                        if (dr.DataRequestType != null)
                        {
                            drtDescription = dr.DataRequestType.RequestDescription;
                        }

                        var eaeDescription = "[EventAutomationEntry.DataRequestType IS NULL]";
                        if (eae.DataRequestType != null)
                        {
                            eaeDescription = eae.DataRequestType.RequestDescription;
                        }

                        this.LogMe("Information", "Updating dataRequest (" + dr.Id + ") from type: " + drtDescription + " to " + eaeDescription, null, threadId);
                        dr.DataRequestTypeId = eae.DataRequestTypeId;
                        resp.UpdateDataRequest(dr, false);
                    }
                }

                resp.UpdateEventAutomationEntry(eae);
            }
            else
            {
                this.LogMe("Information", "IsRunDateComplete = False - No error or completion. RunDate: " + runDate + " | EndDate: " + endDateComplete, null, threadId);
            }

            return errantEae;
        }

        /// <summary>
        ///     Processes Timed EventAutomationEntries.  Entries processed x number of tries (default: 10) will be marked as
        ///     errored.
        /// </summary>
        /// <param name="resp">device Repository</param>
        /// <param name="eae">EventAutomationEntry to process</param>
        /// <returns>If eae is errant, returns the entry for post processing emails</returns>
        private EventAutomationEntry ProcessAutomationEntryTimed(deviceRepository resp, EventAutomationEntry eae, int threadId)
        {
            EventAutomationEntry errantEae = null;
            var device = eae.EventAutomation.Device;

            this.LogMe("Information", "Getting TZR record TAG 19 for SCP file determination (Timed EaeId: " + eae.Id + ")...", null, threadId);
            var ire = resp.GetIntervalReportEntryBySerialDate(device.SerialNumber, eae.AutomationDate, 300);
            if (ire == null)
            {
                //could not find a valid timed event for the time; increment error counter
                this.LogMe("Warning", "Could not find valid HR TZR record for automation type " + EcgServiceEnums.DataRequestType.Timed.ToString() + " (" + device.SerialNumber + " - " + eae.AutomationDate + ")", null, threadId);
                eae.RequestAttempts++;
                if (eae.RequestAttempts > this._maxEaeErrorAttempts)
                {
                    this.LogMe("Error", "Event automation has reached the maximum number of error attempts (" + this._maxEaeErrorAttempts + ") and is now marked as an error.", null, threadId);
                    errantEae = eae;
                    eae.IsError = true;
                }
            }
            else
            {
                this.LogMe("Information", "Found a valid IRE (HR: " + ire.Value + " RecDate: " + ire.RecordDate + ") for event automation: type " + eae.DataRequestTypeId + " (" + device.SerialNumber + " - " + eae.AutomationDate + ")", null, threadId);

                var fileExists = this.CheckIfScpFileExists(device.SerialNumber, ire.SequenceNumber);
                var dr = this.CreateDataRequest(resp, device, eae, ire, fileExists, threadId);
                eae.RequestDate = DateTime.Now;
                eae.IsRequested = true;

                if (fileExists)
                {
                    //Submit to Paceart
                    var eventGuid = this.SubmitEventToPaceart(device, ire, eae.EventAutomation.PatientGuid, resp, EcgServiceEnums.DataRequestType.Timed, eae, threadId);

                    if (eventGuid != Guid.Empty)
                    {
                        this.LogMe("Information", "Event stored in Paceart (Patient: " + eae.EventAutomation.PatientGuid + "EvGuid: " + eventGuid + "), completing data request... ", null, threadId);
                        dr.EventGuid = eventGuid;
                        dr.IsCompleted = true;
                        dr.CompletedAt = DateTime.Now;
                        dr.CompletedBy = DbUserString;
                        resp.UpdateDataRequest(dr, false); //we'll save changes below.
                    }
                    else
                    {
                        this.LogMe("Warning", "EventGuid is empty when submitting " + EcgServiceEnums.DataRequestType.Timed.ToString() + " - dataRequest.Id: " + dr.Id + " to Paceart.", null, threadId);
                    }
                }
            }

            this.LogMe("Information", "Updating EventAutomationEntry...", null, threadId);
            resp.UpdateEventAutomationEntry(eae);

            return errantEae;
        }

        private Guid SubmitEventToPaceart(Device d, TzIntervalReportEntry ire, Guid patientGuid, deviceRepository resp, EcgServiceEnums.DataRequestType drt, EventAutomationEntry eae, int threadId)
        {
            var eventGuid = Guid.Empty;
            short tmpValue = 1;
            var tzeFilename = d.SerialNumber + "_" + this.MakeMe6Digits(ire.SequenceNumber) + "_999.tze";
            var scpFilename = this.BuildScpFilename(d.SerialNumber, ire.SequenceNumber, this._ecgFilePath);
            var va = new VeritéAdapter();

            try
            {
                this.LogMe("Information", "Loading SCP file: " + scpFilename + " ...", null, threadId);
                var scp = new ScpFile(scpFilename);

                var tze = new EventLog();
                tze.CRC = 0;
                tze.CalculatedRecordDate = scp.RecordDate;
                tze.CreateDate = DateTime.Now;
                tze.DataLength = 2;
                tze.DataValue = BitConverter.GetBytes(tmpValue);
                tze.Device = "TZMR";
                tze.EventGuid = Guid.NewGuid();
                //tze.EventStaticPayload;
                tze.FileLength = 92;
                tze.Filename = tzeFilename;
                tze.FirmwareVersion = d.FirmwareVersion;
                tze.FirstFileNumber = ire.SequenceNumber;
                tze.Format = "TZEVT";
                tze.FileRead = 0;
                //tze.ID;
                tze.NumberOfFiles = 1;
                tze.PatientGuid = patientGuid;
                tze.PatientId = patientGuid.ToString();
                tze.RecordDate = ire.RecordDate;
                tze.RecordDateUtc = ire.RecordDateUtc;
                tze.SampleNumber = 0;
                tze.SampleRate = scp.SampleRate; //(short) scp.Signals.RhythmSamplesPerSecond;
                tze.ScpRecordDate = scp.RecordDate;
                tze.SequenceNumber = ire.SequenceNumber;
                tze.SerialNumber = d.SerialNumber;
                tze.Tag = 200;

                this.LogMe("Information", "Creating EventLog entry: " + tzeFilename + " | EventGuid: " + tze.EventGuid, null, threadId);
                resp.CreateEventLog(tze);
                var addEcgRequest = va.ProcessEcgEventFile(tze);

                addEcgRequest.EventDataEx.Long05 = (int)drt; // dra.DataRequest.DataRequestType.RequestType;
                addEcgRequest.EventDataEx.Long06 = (int)eae.Id;
                addEcgRequest.EventDataEx.Text01 = drt.ToString(); //dra.DataRequest.DataRequestType.RequestDescription;
                addEcgRequest.EventDataEx.Text07 = tzeFilename;

                //Check if TimedEvent
                if (drt == EcgServiceEnums.DataRequestType.Timed)
                {
                    //Only set .Bit03 to true if it is a timed event. This will set the event to auto-publish the event report
                    addEcgRequest.EventDataEx.Bit03 = true;
                }

                //Set the trigger type since the above routine doean't have all the necessary data to determine.
                switch (drt)
                {
                    case EcgServiceEnums.DataRequestType.MinimumHr:
                    case EcgServiceEnums.DataRequestType.ProactiveMinimumHr:
                        addEcgRequest.TriggerType = ClinicalEventEnums.EcgEventTriggertype.MinimumHr;
                        break;

                    case EcgServiceEnums.DataRequestType.MaximumHr:
                    case EcgServiceEnums.DataRequestType.ProactiveMaximumHr:
                        addEcgRequest.TriggerType = ClinicalEventEnums.EcgEventTriggertype.MaximumHr;
                        break;

                    case EcgServiceEnums.DataRequestType.Timed:
                        addEcgRequest.TriggerType = ClinicalEventEnums.EcgEventTriggertype.Timed;
                        break;

                    default:
                        addEcgRequest.TriggerType = ClinicalEventEnums.EcgEventTriggertype.DataRequest;
                        break;
                }

                var pa = new PaceartAdapter.PaceartAdapter();

                var response = new AddEcgEventResponse();

                if (this._enablePaceartEventQueue)
                {
                    this.LogMe("Information", "Sending event to paceart (evGuid: " + addEcgRequest.EventGuid + " | RecDate: " + addEcgRequest.RecordDate + " | Queue: " + this._paceartEventQueue + ")...", null, threadId);
                    response = pa.QueueEcgEvent(addEcgRequest);
                }
                else
                {
                    this.LogMe("Information", "Sending event to paceart (evGuid: " + addEcgRequest.EventGuid + " | RecDate: " + addEcgRequest.RecordDate + " | Direct)...", null, threadId);
                    response = pa.AddEcgEvent(addEcgRequest);
                }

                if (response.IsSuccess)
                {
                    eventGuid = response.EventGuid;
                }
                else
                {
                    if (response.InnerException == null)
                    {
                        throw new Exception("VeritéAutomation.SubmitEventToPaceart Error (" + threadId + "): " + response.Message);
                    }
                    else
                    {
                        throw new Exception("VeritéAutomation.SubmitEventToPaceart Error (" + threadId + "): " + response.Message, response.InnerException);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return eventGuid;
        }

        /// <summary>
        ///     Creates a new EventAutomationEntry; used for creating proactive min (4) or proactive max (5) EAEs
        /// </summary>
        /// <param name="resp">device Repository</param>
        /// <param name="ea">EventAutomation parent</param>
        /// <param name="existingEae">Current eventAutomationEntry to use for source data</param>
        /// <param name="dataRequestType">DataRequestTypeId (4=ProactiveMin, 5=ProactiveMax)</param>
        /// <returns>New EventAutomationEntry created</returns>
        private EventAutomationEntry CreateNewEventAutomationEntry(deviceRepository resp, EventAutomation ea, EventAutomationEntry existingEae, EcgServiceEnums.DataRequestType dataRequestType, int threadId)
        {
            this.LogMe("Information", "Creating New EventAutomationEntry: EventAutomationId: " + ea.Id + " | DataRequestType: " + dataRequestType, null, threadId);
            var eae = new EventAutomationEntry();
            eae.AutomationDate = existingEae.AutomationDate;
            eae.CreatedAt = DateTime.Now;
            eae.CreatedBy = DbUserString;
            eae.DataRequestTypeId = (int)dataRequestType;
            eae.IsError = false;
            eae.IsRequested = true;
            eae.Iteration = 0;
            eae.RequestAttempts = 0;
            eae.RequestDate = DateTime.Now;

            this.LogMe("Information", "Linking tables EventAutomation...", null, threadId);
            ea.EventAutomationEntries.Add(eae);
            eae.EventAutomation = ea;
            eae.EventAutomationId = ea.Id;
            //LogMe("Information", "Updating EventAutomation record...", null);
            //resp.CreateEventAutomations(ea);

            return eae;
        }

        /// <summary>
        ///     Creates a dataRequest which includes new entries in the following tables:
        ///     - DataRequest
        ///     - DataRequestActions
        ///     - Actions
        /// </summary>
        /// <param name="resp">deviceRepository</param>
        /// <param name="d">Device record</param>
        /// <param name="eae">EventAutomationEntry record</param>
        /// <param name="ire">TzIntervalReportEntry record (used in grabbing the relevant SCP file number)</param>
        /// <param name="fileAlreadyExists"></param>
        /// <param name="threadId"></param>
        /// <returns>New DataRequest record</returns>
        private DataRequest CreateDataRequest(deviceRepository resp, Device d, EventAutomationEntry eae, TzIntervalReportEntry ire, bool fileAlreadyExists, int threadId)
        {
            var fileId = 0;
            Model.Action a = null;
            DataRequestAction dra = null;

            this.LogMe("Information", "Creating data request...", null, threadId);
            var dr = new Model.DataRequest();
            dr.CompletedAt = null;
            dr.CompletedBy = null;
            dr.CreatedAt = DateTime.Now;
            dr.CreatedBy = DbUserString;
            dr.DataRequestTypeId = eae.DataRequestTypeId;
            dr.DeletedAt = null;
            dr.DeletedBy = null;
            dr.DeviceId = d.Id;
            dr.EventAutomationEntries.Add(eae);
            dr.EventGuid = null;
            dr.HelpdeskTicket = null;
            dr.IsCompleted = false;
            dr.PatientGuid = eae.EventAutomation.PatientGuid;
            dr.RequestedEmail = null;
            dr.RequestScpFilesStart = ire.SequenceNumber;
            dr.RequestScpFilesCount = 1;

            if (!fileAlreadyExists)
            {
                //Only increment FileId if we are submitting an action
                if (d.LastFileId == null)
                {
                    fileId = 0;
                }
                else
                {
                    fileId = (int)d.LastFileId;
                }
                fileId++;
                d.LastFileId = fileId;

                this.LogMe("Information", "Device new lastFileId set: " + fileId, null, threadId);

                this.LogMe("Information", "Creating action...", null, threadId);
                a = new Model.Action();
                a.Format = "TZACT";
                a.DeviceType = "TZMR";
                a.FirmwareVersion = (d.FirmwareVersion == null) ? (byte)20 : (byte)d.FirmwareVersion;
                a.DeviceId = d.Id; //eae.EventAutomation.DeviceId;
                a.DownloadSettings = false;
                a.ClearServerNotReadyFlag = false;
                a.DisplayMessage = null;
                a.RequestSCPFilesStart = ire.SequenceNumber;
                a.RequestSCPFilesCount = 1;
                a.RequestIntervalReportFrom = 0;
                a.RequestIntervalReportCount = 0;
                a.RequestIntervalReportDate = null;
                a.TerminateStudy = false;
                a.FileId = (short)fileId;
                a.CreatedBy = DbUserString;
                a.CreatedDate = DateTime.Now;
                a.DownloadDate = null;
                a.MinimumDeliveryDate = DateTime.Now;
                a.DeletedAt = null;
                a.DeletedBy = null;

                this.LogMe("Information", "Creating data request action...", null, threadId);
                dra = new DataRequestAction();
                dra.Action = a;
                dra.CompletedAt = null;
                dra.CompletedBy = null;
                dra.CreatedAt = DateTime.Now;
                dra.CreatedBy = DbUserString;
                dra.DataRequest = dr;
                dra.DeletedAt = null;
                dra.DeletedBy = null;
                dra.EventLogId = null;
                dra.IsCompleted = false;

                this.LogMe("Information", "Linking action to device...", null, threadId);
                d.Actions.Add(a);
                resp.CreateDataRequestAction(dra, false); //Might need to move this below d.DataRequests.Add(dr)
            }
            else
            {
                dr.IsCompleted = true;
                dr.CompletedAt = DateTime.Now;
                dr.CompletedBy = DbUserString;

                //TODO: Insert Into Paceart?
            }

            this.LogMe("Information", "Linking dataRequest to device...", null, threadId);
            d.DataRequests.Add(dr);

            this.LogMe("Information", "Updating device record...", null, threadId);
            resp.UpdateDevice(d);

            return dr;
        }

        /// <summary>
        ///     Sends an UpdateSettings message to the device with the PatientGuid stored in settings.PatientId
        /// </summary>
        /// <param name="serialNumber">Serial number of the device</param>
        /// <param name="patientGuid">PatientGuid</param>
        private void UpdateDeviceWithPatientId(string serialNumber, Guid patientGuid)
        {
            this.LogMe("Debug", "ENTER: EcgService.Verité.Adapter.VeritéAutomation.UpdateDeviceWithPatientId", null);

            this.LogMe("Information", "Updating device with patientId not enabled!", null);

            //var vdi = new VeritéDeviceInteractions();

            ////1 - Get most recent settings (use logic from device website)
            //LogMe("Debug", "Getting more recent device settings for serial number: " + serialNumber, null);
            //var ds = vdi.GetDeviceSettingsForSerialNumber(serialNumber);

            ////2 - Update PatientId
            //ds.PatientID = patientGuid.ToString();

            ////3 - Write Settings file
            ////4 - Write Action
            //LogMe("Debug", "Saving device settings for serial number: " + serialNumber, null);
            //vdi.SaveDeviceSettingsForSerialNumber(ds, serialNumber);

            this.LogMe("Debug", "EXIT: EcgService.Verité.Adapter.VeritéAutomation.UpdateDeviceWithPatientId", null);
        }

        /// <summary>
        ///     Creates or updates EventAutomationEntry records for the enrollment.
        /// </summary>
        /// <param name="serialNumber">Device serial number</param>
        /// <param name="ped">Paceart Enrollment Dates class</param>
        private void ProcessEventAutomations(string serialNumber, PaceartAdapter.PatientEnrollmentDates ped, AtlasConfigurationItemsWrapper aci)
        {
            this.LogMe("Debug", "ENTER: EcgService.Verité.Adapter.VeritéAutomation.ProcessEventAutomations", null);

            //1 - Get initial items
            //  a - Get Atlas settings
            //  b - Get DeviceId from serial (might be able to put this in the update)
            //  c - Get existing EventAutomations

            //2 - If existing EventAutmations is null

            if (ped.StartDate == DateTime.MinValue || ped.EndDate == DateTime.MinValue)
            {
                throw new Exception("ProcessEventAutomations failed: enrollment start or enddate is blank.");
            }

            //2015-04-07 - mjandes - moved up one method as ACI are used by VIA as well
            ////  a - Get Atlas settings
            //LogMe("Debug", "Getting atlas items for patientGuid: " + ped.PatientGuid, null);
            //var aci = GetAtlasItems(ped.PatientGuid);

            //  b - Get DeviceId from serial (might be able to put this in the update)
            var resp = new deviceRepository();
            this.LogMe("Debug", "Getting device by serial number for serial: " + serialNumber, null);
            var device = resp.GetDeviceBySerialNumber(serialNumber);

            //  c - Get existing EventAutomations
            this.LogMe("Debug", "Getting existing event automations... ", null);

            //2014-12-22 - mjandes
            //We need to look for eventAutomations by patientguid in case there is a replacement device sent
            var ea = resp.GetEventAutomationByPatientguid(ped.PatientGuid);
            //var ea = resp.GetEventAutomationByPatientguidDeviceId(ped.PatientGuid, device.Id);

            if (ea == null)
            {
                this.LogMe("Debug", "No existing event automations found.", null);

                ea = new Model.EventAutomation();
                ea.CreatedAt = DateTime.Now;
                ea.CreatedBy = "deviceMSMQ";
                ea.DeviceId = device.Id;
                ea.EndDate = ped.EndDate;
                ea.IsActive = true;
                ea.PatientGuid = ped.PatientGuid;
                ea.StartDate = ped.StartDate;

                this.AddEventAutomationEntriesToParent(ea, ped.StartDate, ped.EndDate, aci, 0);

                resp.CreateEventAutomations(ea);
            }
            else
            {
                this.LogMe("Debug", "Existing event automations found; checking to see if updates are required.", null);

                //Check to see if enrollment changed; check to see if timed changed (need to query eae for timed only)
                //Delete any days with requests that haven't been made
                //Check for new max eae date and max enrollment date

                //We have data, lets check to see if we need to delete anything

                //2014-12-22 - mjandes - Check for serial number change.
                if (device.Id != ea.DeviceId)
                {
                    ea.DeviceId = device.Id;
                }

                //TODO: Figure out AtlasSettings check
                if (ea.EndDate != ped.EndDate)
                {
                    if (ped.EndDate < ea.EndDate)
                    {
                        //TODO: Check and delete anything that hasn't been requested or errored yet
                    }

                    if (ped.EndDate > ea.EndDate)
                    {
                        var xStartDate = (DateTime)ea.EndDate;
                        var dayCounter = 0;

                        if (ped.StartDate > ea.EndDate)
                        {
                            xStartDate = ped.StartDate;
                        }
                        else
                        {
                            var ts = (TimeSpan)(ea.EndDate - ped.StartDate);
                            dayCounter = ts.Days;
                        }

                        this.AddEventAutomationEntriesToParent(ea, xStartDate, ped.EndDate, aci, dayCounter);
                    } //if (ped.EndDate > ea.EndDate)
                } //if (ea.EndDate != ped.EndDate)

                resp.SaveChanges(); //2014-12-22 - mjandes - call a save to ensure the serial number changes.
            } // else if (ea == null)

            this.LogMe("Debug", "EXIT: EcgService.Verité.Adapter.VeritéAutomation.ProcessEventAutomations", null);

            //TODO: add more TODO messages!
        }

        /// <summary>
        ///     Checks the EventLog and TzIntervalReport tables for incorrect patientGuids for the device and updates them
        /// </summary>
        /// <param name="serialNumber">Devcie serial number</param>
        /// <param name="ped">Paceart Enrollment Dates class</param>
        private void CheckAndUpdateExistingDefaultRecords(string serialNumber, PaceartAdapter.PatientEnrollmentDates ped)
        {
            this.LogMe("Warning", "CheckAndUpdateExistingDefaultRecords NOT implemented yet", null);
            //TODO: Bitch this one to add
            //make sure to do both TZR & EventLog!!
            //do we need to query the inventory service to determine the actual time the patient had the device from?
        }

        private void AddEventAutomationEntriesToParent(EventAutomation ea, DateTime startDate, DateTime endDate, AtlasConfigurationItemsWrapper aci, int dayCounter)
        {
            this.LogMe("Debug", "ENTER: EcgService.Verité.Adapter.VeritéAutomation.AddEventAutomationEntriesToParent", null);

            //var dayCounter = 0;
            //for (var d = ped.StartDate; d < ped.EndDate; d = d.AddDays(1))
            for (var d = startDate; d < endDate; d = d.AddDays(1))
            {
                var eae = new EventAutomationEntry();
                eae.CreatedAt = DateTime.Now;
                eae.CreatedBy = "deviceMSMQ";
                eae.DataRequestTypeId = (int)EcgServiceEnums.DataRequestType.MinimumHr;
                eae.AutomationDate = d;
                ea.EventAutomationEntries.Add(eae);

                eae = new EventAutomationEntry();
                eae.CreatedAt = DateTime.Now;
                eae.CreatedBy = "deviceMSMQ";
                eae.DataRequestTypeId = (int)EcgServiceEnums.DataRequestType.MaximumHr;
                eae.AutomationDate = d;
                ea.EventAutomationEntries.Add(eae);

                //IF Atlas settings have timed
                if (aci.EnableTimedEvents)
                {
                    if (dayCounter % aci.TimedDailyFrequency == 0)
                    {
                        this.LogMe("Debug", "Adding timed event(s) for " + d, null);

                        //2014-10-13 - mjandes - We determine how many hours we need to increment between timed strips if they want more than 1
                        //for example, 2 timed strips per days = 12 hours apart; 4 timed strips per day = 6 hours apart
                        //we then do a for loop incrementing through the timed strips per day and adding hour incrementor to each value.
                        var hourIncrementor = 0;
                        switch (aci.TimedStripsPerDay)
                        {
                            case 0:
                            case 1:
                                hourIncrementor = 0;
                                break;

                            case 2:
                            case 3:
                                hourIncrementor = 12;
                                break;

                            case 4:
                            case 5:
                                hourIncrementor = 6;
                                break;

                            case 6:
                            case 7:
                            case 8:
                            case 9:
                            case 10:
                            case 11:
                                hourIncrementor = 4;
                                break;

                            case 24:
                                hourIncrementor = 1;
                                break;

                            default:
                                hourIncrementor = 2;
                                break;
                        }

                        var timedStripHour = aci.TimedStripHour;
                        for (int i = 0; i < aci.TimedStripsPerDay; i++)
                        {
                            eae = new EventAutomationEntry();
                            eae.CreatedAt = DateTime.Now;
                            eae.CreatedBy = "deviceMSMQ";
                            eae.DataRequestTypeId = (int)EcgServiceEnums.DataRequestType.Timed;
                            eae.AutomationDate = d.AddHours(timedStripHour);
                            ea.EventAutomationEntries.Add(eae);

                            timedStripHour += hourIncrementor;
                        }

                        //2014-10-13 - mjandes - old way below.
                        //eae = new EventAutomationEntry();
                        //eae.CreatedAt = DateTime.Now;
                        //eae.CreatedBy = "deviceMSMQ";
                        //eae.DataRequestTypeId = (int) EcgServiceEnums.DataRequestType.Timed;
                        //eae.AutomationDate = d.AddHours(aci.TimedStripHour);
                        //ea.EventAutomationEntries.Add(eae);

                        //if (aci.TimedStripsPerDay > 1) {
                        //    eae = new EventAutomationEntry();
                        //    eae.CreatedAt = DateTime.Now;
                        //    eae.CreatedBy = "deviceMSMQ";
                        //    eae.DataRequestTypeId = (int) EcgServiceEnums.DataRequestType.Timed;
                        //    eae.AutomationDate = d.AddHours(aci.TimedStripHour + 12);
                        //    ea.EventAutomationEntries.Add(eae);
                        //}
                    }
                }

                dayCounter++;
            }

            this.LogMe("Debug", "EXIT: EcgService.Verité.Adapter.VeritéAutomation.AddEventAutomationEntriesToParent", null);
        }

        private AtlasConfigurationItemsWrapper GetAtlasItems(Guid patientGuid)
        {
            this.LogMe("Debug", "ENTER: EcgService.Verité.Adapter.VeritéAutomation.GetAtlasItems", null);

            var aci = new AtlasConfigurationItemsWrapper();

            var client = new AtlasClient();
            if (this._domain != string.Empty && this._domainUsername != string.Empty && this._domainPassword != string.Empty)
            {
                client.SetWindowsCredentials(this._domainUsername, this._domainPassword, this._domain);
            }

            //NOTE: Use names for references
            var application = ApplicationReference.ByName("Reporting");
            var area = AreaReference.ByName(application, "Default");
            var level = HierarchyLevelReference.ByName(area, "Patient");
            var node = HierarchyNodeReference.ByExternalKey(level, patientGuid.ToString());
            var result = client.GetCurrentValues(node);
            var tmpB = false;
            var tmpI = 0;
            foreach (var resultRecord in result)
            {
                switch (resultRecord.Setting.Name)
                {
                    case "Timed":
                        tmpB = false;
                        if (bool.TryParse(resultRecord.Value, out tmpB))
                        {
                            aci.EnableTimedEvents = tmpB;
                        }
                        else
                        {
                            aci.EnableTimedEvents = false;
                        }
                        break;

                    case "TimedDailyFrequency":
                        tmpI = 1;
                        aci.TimedDailyFrequency = int.TryParse(resultRecord.Value, out tmpI) ? tmpI : 1;
                        break;

                    case "TimedStripsPerDay":
                        tmpI = 1;
                        aci.TimedStripsPerDay = int.TryParse(resultRecord.Value, out tmpI) ? tmpI : 1;
                        break;

                    case "TimedStripHour":
                        tmpI = 6;
                        var tmpS = resultRecord.Value;

                        //2014-07-31 - mjandes
                        //TimedStripHour is stored as HH:MM but we are expecting an integer HH.
                        if (resultRecord.Value.Length > 2 && resultRecord.Value.Contains(":"))
                        {
                            //assume that this is the format of HH:MM instead of HH
                            //012345
                            //00:00

                            tmpS = resultRecord.Value.Substring(0, resultRecord.Value.IndexOf(':'));
                        }

                        //aci.TimedStripHour = int.TryParse(resultRecord.Value, out tmpI) ? tmpI : 6;
                        aci.TimedStripHour = int.TryParse(tmpS, out tmpI) ? tmpI : 6;
                        break;

                    case "EnableViaReport":
                        //2015-04-07 - mjandes
                        tmpB = false;
                        if (bool.TryParse(resultRecord.Value, out tmpB))
                        {
                            aci.EnableViaReport = tmpB;
                        }
                        else
                        {
                            aci.EnableViaReport = false;
                        }
                        break;

                    case "EnableViaFullDisclosure":
                        //2015-04-07 - mjandes
                        tmpB = false;
                        if (bool.TryParse(resultRecord.Value, out tmpB))
                        {
                            aci.EnableViaFullDisclosure = tmpB;
                        }
                        else
                        {
                            aci.EnableViaFullDisclosure = false;
                        }
                        break;
                }
            }

            //Check to make sure more than 2 per day is not requested, if it is, set to our 2-per-day max.
            //if (aci.TimedStripsPerDay > 2)
            //    aci.TimedStripsPerDay = 2;
            //2014-10-13 - mjandes - Alter timedStripsPerDay to support 1-24, we are only allowing divisors of 24 (1,2,4,6,12,24) where
            // the number doesn't shift the timed event am/pm to different times (i.e. 3,8 wont work). If the number is not one of these, round down.
            switch (aci.TimedStripsPerDay)
            {
                case 0:
                case 1:
                    //do nothing
                    break;

                case 2:
                case 3:
                    aci.TimedStripsPerDay = 2;

                    if (aci.EnableTimedEvents && aci.TimedStripHour > 11)
                    {
                        aci.TimedStripHour = aci.TimedStripHour - 12;
                    }
                    break;

                case 4:
                case 5:
                    if (aci.EnableTimedEvents && aci.TimedStripHour > 5)
                    {
                        //if we have the timed event set higher than 5 (the max number in the first quadrant - 6-hour group)
                        //then determin the cooresponding value in the quadrant (0-5) to properly set the start time
                        //so that we always have an equal number of timed per day and they all start at the same relative time.
                        aci.TimedStripHour = aci.TimedStripHour % 6;
                    }
                    aci.TimedStripsPerDay = 4;
                    break;

                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    if (aci.EnableTimedEvents && aci.TimedStripHour > 3)
                    {
                        //same as case 4/5 above but with 6 4-hour groups instead of 4 groups
                        aci.TimedStripHour = aci.TimedStripHour % 4;
                    }
                    aci.TimedStripsPerDay = 6;
                    break;

                case 24:
                    if (aci.EnableTimedEvents && aci.TimedStripHour > 0)
                    {
                        aci.TimedStripHour = 0; //force to start at midnight, only start point to get 24 in a day.
                    }
                    break;

                default:
                    aci.TimedStripsPerDay = 12;
                    if (aci.EnableTimedEvents && aci.TimedStripHour > 1)
                    {
                        aci.TimedStripHour = aci.TimedStripHour % 2; //if even start at 0, else start on 1am.
                    }
                    break;
            }

            //2014-10-13 - mjandes - aci.TimedStripHour adjustment is done above.
            //Check to make sure we dont have Timed=true, TimedStripsPerDay=2 and TimedStripHour>12 (i.e. starting in PM)
            //if (aci.EnableTimedEvents && aci.TimedStripsPerDay > 1 && aci.TimedStripHour > 11) {
            //    aci.TimedStripHour = aci.TimedStripHour - 12;
            //}

            this.LogMe("Debug", "EXIT: EcgService.Verité.Adapter.VeritéAutomation.GetAtlasItems", null);
            return aci;
        }

        #region Helpers

        private bool CheckIfScpFileExists(string serialNumber, int scpFileNumber)
        {
            var rtn = false;
            var s = this.BuildScpFilename(serialNumber, scpFileNumber, this._ecgFilePath);
            if (File.Exists(s))
            {
                rtn = true;
            }
            else
            {
                rtn = false;
            }

            return rtn;
        }

        private string BuildScpFilename(string serial, int filenum, string path)
        {
            this.LogMe("Debug", "BuildScpFilename Inputs -- Serial: " + serial + " | filenum: " + filenum + " | path: " + path, null);
            if (!path.EndsWith("\\"))
            {
                path += "\\";
            }

            //2013-09-24 -- put files under serial directory to cut down on number of files in a directory
            //ignore the BOXCAR (keep in root SCP directory)
            if (serial.ToUpper() != "BOXCAR")
            {
                path += serial + "\\";
            }

            return path + serial + "_" + this.MakeMe6Digits(filenum) + ".scp";
        }

        private string MakeMe6Digits(int i)
        {
            //2014-07-09 - there was an issue where a manual/triggered event that was sent with sequence number 0 would try to look for sequenceNumber -1
            //which would error out this routine.  We are returning file 999999 instead which won't be found and will trigger a boxcar for the first
            //file.

            var s = "999999";
            if (i < 0)
            {
                this.LogMe("Warning", "input number <0; setting to 999999 to trigger boxcar - i: " + i, null);
            }
            else
            {
                s = (i + 1000000).ToString().Substring(1, 6);
            }

            return s;
        }

        private DateTime GetDateToUseForPatientDetermination(DateTime recordDate, DateTime queueDate)
        {
            this.LogMe("Debug", "ENTER: EcgService.Verité.Adapter.VeritéAutomation.GetDateToUseForPatientDetermination", null);

            //Use recordDate for patient determination UNLESS we have a minimum accepted date and it is <= to that date, then use
            //the date we wrote the queue (which should be the date we received the message)
            var rtn = recordDate;

            if (this._minimumDate > DateTime.MinValue)
            {
                if (recordDate <= this._minimumDate)
                {
                    rtn = queueDate;
                }
            }

            this.LogMe("Debug", "EXIT: EcgService.Verité.Adapter.VeritéAutomation.GetDateToUseForPatientDetermination", null);
            return rtn;
        }

        private void LoadServiceData()
        {
            this.LogMe("Debug", "Enter Procedure: AeraAdapter.LoadServiceData", null);

            //load app.config variables here
            //Note: _defaultPatientGuid is only populated here for logging purposes.
            this._defaultPatientGuid = Guid.Empty;
            if (ConfigurationManager.AppSettings["DefaultPatientGuid"] != null && ConfigurationManager.AppSettings["DefaultPatientGuid"] != string.Empty)
            {
                if (!Guid.TryParse(ConfigurationManager.AppSettings["DefaultPatientGuid"], out this._defaultPatientGuid))
                {
                    this._defaultPatientGuid = Guid.Empty;
                }
            }

            //2013-10-31
            this._enableViaAutoRequest = false;
            if (ConfigurationManager.AppSettings["EnableViaAutoRequest"] != null && ConfigurationManager.AppSettings["EnableViaAutoRequest"].ToUpper() == "TRUE")
            {
                this._enableViaAutoRequest = true;
            }

            //2014-07-04
            if (ConfigurationManager.AppSettings["MinimumDate"] != null && ConfigurationManager.AppSettings["MinimumDate"] != string.Empty)
            {
                if (!DateTime.TryParse(ConfigurationManager.AppSettings["MinimumDate"], out this._minimumDate))
                {
                    this._minimumDate = DateTime.MinValue;
                }
            }

            if (ConfigurationManager.AppSettings["MaximumEventAutomationErrorAttempts"] != null && ConfigurationManager.AppSettings["MaximumEventAutomationErrorAttempts"] != string.Empty)
            {
                if (!int.TryParse(ConfigurationManager.AppSettings["MaximumEventAutomationErrorAttempts"], out this._maxEaeErrorAttempts))
                {
                    this._maxEaeErrorAttempts = 100;
                }
            }

            if (ConfigurationManager.AppSettings["AddHoursForMaxDateForMinMaxAutomation"] != null && ConfigurationManager.AppSettings["AddHoursForMaxDateForMinMaxAutomation"] != string.Empty)
            {
                if (!int.TryParse(ConfigurationManager.AppSettings["AddHoursForMaxDateForMinMaxAutomation"], out this._addHourToDateForMinMaxComplete))
                {
                    this._addHourToDateForMinMaxComplete = 2;
                }
            }

            if (ConfigurationManager.AppSettings["DomainUserName"] != null && ConfigurationManager.AppSettings["DomainUserName"] != string.Empty)
            {
                this._domainUsername = ConfigurationManager.AppSettings["DomainUserName"];
            }

            if (ConfigurationManager.AppSettings["DomainPassword"] != null && ConfigurationManager.AppSettings["DomainPassword"] != string.Empty)
            {
                this._domainPassword = ConfigurationManager.AppSettings["DomainPassword"];
            }

            if (ConfigurationManager.AppSettings["Domain"] != null && ConfigurationManager.AppSettings["Domain"] != string.Empty)
            {
                this._domain = ConfigurationManager.AppSettings["Domain"];
            }

            this._ecgFilePath = ConfigurationManager.AppSettings["deviceEcgFilePath"];

            if (ConfigurationManager.AppSettings["deviceAutomationQueue"] != null)
            {
                this._queue = ConfigurationManager.AppSettings["deviceAutomationQueue"];
            }

            if (ConfigurationManager.AppSettings["deviceAutomationItemQueue"] != null)
            {
                this._deviceAutomationItemQueue = ConfigurationManager.AppSettings["deviceAutomationItemQueue"];
            }

            if (ConfigurationManager.AppSettings["OrderEventAutomationEntries"] != null && ConfigurationManager.AppSettings["OrderEventAutomationEntries"].ToUpper() == "TRUE")
            {
                this._orderEventAutomationEntries = true;
            }

            if (ConfigurationManager.AppSettings["PaceartEventsQueue"] != null)
            {
                this._paceartEventQueue = ConfigurationManager.AppSettings["PaceartEventsQueue"].ToString();
            }

            if (ConfigurationManager.AppSettings["EnablePaceartEventsQueue"] != null && ConfigurationManager.AppSettings["EnablePaceartEventsQueue"].ToUpper() == "TRUE")
            {
                if (this._paceartEventQueue != string.Empty)
                {
                    this._enablePaceartEventQueue = true;
                    this.LogMe("Information", "Paceart Events Queue: " + this._paceartEventQueue, null);
                }
                else
                {
                    this.LogMe("Warning", "Paceart events queue is blank and wont be enabled.", null);
                }
            }

            //OTA
            if (ConfigurationManager.AppSettings["OtaQueue"] != null)
            {
                this._otaQueue = ConfigurationManager.AppSettings["OtaQueue"];
            }

            if (ConfigurationManager.AppSettings["OtaArchiveRoot"] != null)
            {
                this._otaArchiveRoot = ConfigurationManager.AppSettings["OtaArchiveRoot"];
            }

            //Load config values for sending email messages on errors
            this.LoadServiceDataEmail();

            this.LogMe("Debug", "Exit Procedure: AeraAdapter.LoadServiceData", null);
        }

        private void LoadServiceDataEmail()
        {
            this.LogMe("Debug", "Enter Procedure: AeraAdapter.LoadServiceDataEmail", null);

            if (ConfigurationManager.AppSettings["SendEmailOnError"] != null && ConfigurationManager.AppSettings["SendEmailOnError"].ToUpper() == "TRUE")
            {
                this._sendEmailOnError = true;
            }

            if (ConfigurationManager.AppSettings["SendEmailTo"] != null)
            {
                try
                {
                    string tmpS = ConfigurationManager.AppSettings["SendEmailTo"].ToString();
                    if (tmpS != string.Empty)
                    {
                        if (tmpS.Contains(";"))
                        {
                            char[] tmpC = new char[1]
                                        {
                                            ';'
                                        };
                            foreach (var email in tmpS.Split(tmpC, Int32.MaxValue))
                            {
                                this._sendEmailTo.Add(email);
                            }
                        }
                        else
                        {
                            this._sendEmailTo.Add(tmpS);
                        }
                    }
                }
                catch (Exception)
                {
                    this._sendEmailTo = new List<string>();
                }
            }

            if (ConfigurationManager.AppSettings["SendEmailServer"] != null)
            {
                this._sendEmailServer = ConfigurationManager.AppSettings["SendEmailServer"].ToString();
            }

            if (ConfigurationManager.AppSettings["SendEmailUsername"] != null)
            {
                this._sendEmailUsername = ConfigurationManager.AppSettings["SendEmailUsername"].ToString();
            }

            if (ConfigurationManager.AppSettings["SendEmailPassword"] != null)
            {
                this._sendEmailPassword = ConfigurationManager.AppSettings["SendEmailPassword"].ToString();
            }

            if (ConfigurationManager.AppSettings["SendEmailFrom"] != null)
            {
                this._sendEmailFrom = ConfigurationManager.AppSettings["SendEmailFrom"].ToString();
            }

            if (ConfigurationManager.AppSettings["SendEmailEnableSsl"] != null && ConfigurationManager.AppSettings["SendEmailEnableSsl"].ToUpper() == "TRUE")
            {
                this._sendEmailEnableSsl = true;
            }

            if (ConfigurationManager.AppSettings["SendEmailSubject"] != null && ConfigurationManager.AppSettings["SendEmailSubject"] != string.Empty)
            {
                this._sendEmailSubject = ConfigurationManager.AppSettings["SendEmailSubject"].ToString();
            }

            int tmpI = 0;
            if (ConfigurationManager.AppSettings["SendEmailServerPort"] != null && int.TryParse(ConfigurationManager.AppSettings["SendEmailServerPort"].ToString(), out tmpI))
            {
                this._sendEmailPort = tmpI;
            }

            //check to make sure we have a server and a to
            if (this._sendEmailOnError)
            {
                if (this._sendEmailTo.Count == 0 || this._sendEmailServer == string.Empty)
                {
                    this.LogMe("Information", "EmailTo or EmailServer is blank; emailing disabled.", null);
                }
            }

            this.LogMe("Debug", "Exit Procedure: AeraAdapter.LoadServiceDataEmail", null);
        }

        // NOTE: this attribute causes the compiler to not emit method calls to LogMe unless the DEBUG symbol is defined
        // [System.Diagnostics.Conditional("DEBUG")]
        //private void LogMe(LogMessageLevel level, string msg, Exception error) {
        //    try
        //    {
        //        if (error == null)
        //        {
        //            LogManager.Log(level, null, msg);
        //        }
        //        else
        //        {
        //            LogManager.Log(level, null, msg, error);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}
        private void LogMe(string level, string msg, Exception error)
        {
            try
            {
                switch (level.ToUpper())
                {
                    case "ERROR":
                        if (error == null)
                        {
                            Log.Error(msg);
                        }
                        else
                        {
                            Log.Error(msg, error);
                        }
                        break;

                    case "FATALERROR":
                    case "FATAL":
                        if (error == null)
                        {
                            Log.Fatal(msg);
                        }
                        else
                        {
                            Log.Fatal(msg, error);
                        }
                        break;

                    case "INFORMATION":
                    case "INFO":
                        if (error == null)
                        {
                            Log.Info(msg);
                        }
                        else
                        {
                            Log.Info(msg, error);
                        }
                        break;

                    case "WARNING":
                    case "WARN":
                        if (error == null)
                        {
                            Log.Warn(msg);
                        }
                        else
                        {
                            Log.Warn(msg, error);
                        }
                        break;

                    case "DEBUG":
                    default:
                        if (error == null)
                        {
                            Log.Debug(msg);
                        }
                        else
                        {
                            Log.Debug(msg, error);
                        }
                        break;
                }
            }
            catch (Exception x)
            {
                //try 1 more time to log this
                try
                {
                    Log.Error(msg, x);
                }
                catch (Exception)
                {
                }
            }
        }

        //private void LogMe(LogMessageLevel level, string msg, Exception error, int threadId)
        //{
        //    try {
        //        var msg2 = threadId.ToString() + " - " + msg;
        //        if (error == null)
        //        {
        //            LogManager.Log(level, null, msg2);
        //        }
        //        else
        //        {
        //            LogManager.Log(level, null, msg2, error);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}
        private void LogMe(string level, string msg2, Exception error, int threadId)
        {
            var msg = threadId.ToString() + " - " + msg2;

            try
            {
                switch (level.ToUpper())
                {
                    case "ERROR":
                        if (error == null)
                        {
                            Log.Error(msg);
                        }
                        else
                        {
                            Log.Error(msg, error);
                        }
                        break;

                    case "FATALERROR":
                    case "FATAL":
                        if (error == null)
                        {
                            Log.Fatal(msg);
                        }
                        else
                        {
                            Log.Fatal(msg, error);
                        }
                        break;

                    case "INFORMATION":
                    case "INFO":
                        if (error == null)
                        {
                            Log.Info(msg);
                        }
                        else
                        {
                            Log.Info(msg, error);
                        }
                        break;

                    case "WARNING":
                    case "WARN":
                        if (error == null)
                        {
                            Log.Warn(msg);
                        }
                        else
                        {
                            Log.Warn(msg, error);
                        }
                        break;

                    case "DEBUG":
                    default:
                        if (error == null)
                        {
                            Log.Debug(msg);
                        }
                        else
                        {
                            Log.Debug(msg, error);
                        }
                        break;
                }
            }
            catch (Exception x)
            {
                //try 1 more time to log this
                try
                {
                    Log.Error(msg, x);
                }
                catch (Exception)
                {
                }
            }
        }

        #endregion Helpers

        #endregion Private Methods

        #region OTA Queue

        /// <summary>
        ///     Adds a message to the OTA queue for OTA filestore cleanup
        /// </summary>
        /// <param name="patientGuid">Paceart Guid (Master.Guid) of patient - used for directory structure.</param>
        /// <param name="serialNumber">Serial number of device.</param>
        /// <param name="sdArchiveDate">All files before this date matching the serial number will be archived.</param>
        public void PrepareAndQueueOTA(Guid patientGuid, string serialNumber, DateTime sdArchiveDate)
        {
            if (this._otaArchiveRoot == string.Empty || this._otaQueue == string.Empty)
            {
                Log.Warn("Warning, OTA archive queue disabled due to missing settings in the config file.");
            }
            else
            {
                Log.Debug("Preparing OTA message for queue (patientGuid: " + patientGuid + " - Serial: " + serialNumber);
                var enrollment = new EnrolmentDto();

                enrollment.ArchiveDestination = this.BuildArchiveDestination(serialNumber, patientGuid, this._otaArchiveRoot);
                enrollment.SerialNumber = serialNumber;
                enrollment.EnrolmentId = patientGuid.ToString();
                enrollment.SdCardArchiveTime = sdArchiveDate;

                this.SaveEnrollmentToQueue(enrollment, this._otaQueue);
            }
        }

        private string BuildArchiveDestination(string serial, Guid patientGuid, string outputDirectory)
        {
            var rtn = new StringBuilder();
            rtn.Append(outputDirectory);

            if (!outputDirectory.EndsWith("\\"))
            {
                rtn.Append("\\");
            }

            rtn.Append(serial);
            rtn.Append("\\");

            if (patientGuid == this._defaultPatientGuid || patientGuid == Guid.Empty)
            {
                rtn.Append(this.GetDateForFilename(DateTime.Now));
            }
            else
            {
                rtn.Append(patientGuid.ToString());
            }
            //rtn.Append("\\");

            return rtn.ToString();
        }

        private string GetDateForFilename(DateTime dt)
        {
            var rtn = new StringBuilder();

            rtn.Append(dt.Year);
            rtn.Append((dt.Month + 100).ToString().Substring(1, 2));
            rtn.Append((dt.Day + 100).ToString().Substring(1, 2));
            rtn.Append("_");
            rtn.Append((dt.Hour + 100).ToString().Substring(1, 2));
            rtn.Append((dt.Minute + 100).ToString().Substring(1, 2));
            rtn.Append((dt.Second + 100).ToString().Substring(1, 2));

            return rtn.ToString();
        }

        private void SaveEnrollmentToQueue(EnrolmentDto enrollment, string queueName)
        {
            try
            {
                var ser = new DataContractJsonSerializer(typeof(EnrolmentDto));

                var stream1 = new MemoryStream();
                ser.WriteObject(stream1, enrollment);
                stream1.Position = 0;

                var msgTx = new MessageQueueTransaction();

                var message = new Message();
                message.Label = enrollment.SerialNumber;
                message.BodyStream = stream1;

                var msgQ = new MessageQueue(queueName);

                msgTx.Begin();
                msgQ.Send(message, MessageQueueTransactionType.Single);
                msgTx.Commit();

                Log.Info("- Message added to OTA queue");
                Log.Info("+ Serial: " + enrollment.SerialNumber);
                Log.Info("+ Archive Destination: " + enrollment.ArchiveDestination);
                Log.Info("+ Enrollment ID: " + enrollment.EnrolmentId);
                Log.Info("+ Archive Date: " + enrollment.SdCardArchiveTime);
            }
            catch (Exception x)
            {
                throw;
            }
        }

        #endregion OTA Queue
    }
}