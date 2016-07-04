using log4net;
using Revampness.Services.Contracts;
using Revampness.Services.device.Model;
using System;
using System.Collections.Generic;
using System.Messaging;
using System.Reflection;
using System.Threading;

namespace TsmRptLibrary
{
    internal class Tag1Processor
    {
        private readonly IConfigHelper _configHelper = new ConfigHelper();
        private readonly List<EcgServiceEnums.DataRequestType> _dataRequestTypeList = new List<EcgServiceEnums.DataRequestType>();

        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IPaceartAdapter _paceartAdapter = new PaceartAdapterWrapper();
        private readonly IdeviceRepository _deviceRepository = new deviceRepositoryWrapper();
        private IAtlasConfigurationItems _atlasConfigItems;
        private EventAutomationWrapper _eventAutomation;
        private EventAutomationEntryWrapper _eventAutomationEntry;
        private Message _message;
        private PatientEnrollmentDatesWrapper _patientEnrollmentInfo;
        private Guid _patientGuid;
        private int _retryCounter;
        private string _serialNumber;
        private IStartupAutomation _startupAutomationMessage;
        private string _startupAutomationMsgFilename;

        private DateTime WhenThisMessageProcessingStarted { get; } = DateTime.Now;

        public void HandleExistingAutomation()
        {
            if (!this._patientEnrollmentInfo.DidEnrollmentDurationChange(this._eventAutomation.EndDate))
            {
                if (this._patientEnrollmentInfo.IsEnrollmentExtended(this._eventAutomation.EndDate))
                {
                    this._eventAutomationEntry.DeleteOldEventAutomationEntryData(this._serialNumber);
                    this._eventAutomation.CreateEventAutomationEntryRecords(this._dataRequestTypeList, hasExistingAutomations: true);
                }
            }

            this._eventAutomation.SaveEventAutomations();
        }

        public void HandleNonExistingAutomation()
        {
            this._eventAutomation.InitializeMembers();
            this._eventAutomation.CreateEventAutomationEntryRecords(this._dataRequestTypeList, hasExistingAutomations: false);
            this._eventAutomation.SaveEventAutomations();
        }

        public bool IsOurProcessOld()
        {
            this._startupAutomationMessage = this._message.DeserializeToJson<IStartupAutomation>();
            var isOurProcessOld = this._startupAutomationMessage.LastAttemptDate > this.WhenThisMessageProcessingStarted;
            if (!isOurProcessOld) return false;
            this._logger.Warn("Our process is old. Stopping.");
            return true;
        }

        public void Start()
        {
            var messageQueue = new MessageQueueWrapper(this._configHelper.deviceAutomationQueueLocation);

            while (!messageQueue.IsQueueEmpty())
            {
                if (this.IsMessageQueueException(messageQueue))
                {
                    break;
                }

                if (this._message == null)
                {
                    break;
                }

                if (this.IsOurProcessOld())
                {
                    break;
                }

                this.ProcessAndReturnMessageOnError(messageQueue);
            }
        }

        protected ILog GetLogger()
        {
            return LogManager.GetLogger(this.GetType().Assembly, this.GetType().Name + '.' + Thread.CurrentThread.ManagedThreadId);
        }

        private void CreateParentAndChildAutomations()
        {
            this._eventAutomation = new EventAutomationWrapper(this._configHelper, this._deviceRepository, this._patientEnrollmentInfo,
                this._atlasConfigItems);
            this._eventAutomationEntry = new EventAutomationEntryWrapper(this._atlasConfigItems, this._patientEnrollmentInfo);
            this._eventAutomation.DeviceId = this._deviceRepository.GetDeviceIdForSerialNumber(this._serialNumber);
        }

        private void GetAtlasConfigItems()
        {
            this._atlasConfigItems = new AtlasConfigurationItemsWrapper(this._configHelper, new AtlasClientWrapper(), this._patientGuid);
            this._atlasConfigItems.FetchAtlasSettings();
            this._atlasConfigItems.UpdatePropertiesBasedOnAtlasSettings();
            this._atlasConfigItems.UpdateTimedStrips();
        }

        private void HandleAllAutomations()
        {
            if (!this._patientEnrollmentInfo.HasEventAutomations())
            {
                this.HandleNonExistingAutomation();
            }
            else
            {
                this.HandleExistingAutomation();
            }
        }

        private StartupAutomation IncrementRetryCounter(IStartupAutomation startupAutomation)
        {
            var updatedMessage = new StartupAutomation();

            updatedMessage.Tze = startupAutomation.Tze;
            updatedMessage.OriginalQueueDate = startupAutomation.OriginalQueueDate;
            updatedMessage.LastAttemptDate = DateTime.Now;
            updatedMessage.RetryCounter += 1;

            return updatedMessage;
        }

        private void InitializePrivateFieldsFromMessage()
        {
            this._serialNumber = this._startupAutomationMessage.Tze.SerialNumber;
            this._startupAutomationMsgFilename = this._startupAutomationMessage.Tze.Filename;
            this._patientGuid = this._startupAutomationMessage.Tze.PatientGuid.ToGuid();
            this._patientEnrollmentInfo = new PatientEnrollmentDatesWrapper(this._paceartAdapter, this._serialNumber, this._deviceRepository);
        }

        private bool? IsValidEnrollment()
        {
            if (this._patientEnrollmentInfo.EnrollmentFound == false)
            {
                return null;
            }

            return this._patientEnrollmentInfo.AreDatesValid;
        }

        private void ProcessAndReturnMessageOnError(MessageQueueWrapper messageQueue)
        {
            try
            {
                this.InitializePrivateFieldsFromMessage();

                if (this.IsValidEnrollment() == false)
                {
                    return;
                }

                if (this.IsValidEnrollment() == null)
                {
                    throw new Exception();
                }

                this.ProcessEntireMessage();

                if (!messageQueue.IsAbleToPop())
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                this.ReturnMessageToQueue(messageQueue, this._startupAutomationMessage);
                throw;
            }
        }

        private void ProcessEntireMessage()
        {
            this.GetAtlasConfigItems();

            this.CreateParentAndChildAutomations();

            this.HandleAllAutomations();
        }

        private void ReturnMessageToQueue(IMessageQueue messageQueue, IStartupAutomation startupAutomation)
        {
            var startupMessage = this.IncrementRetryCounter(startupAutomation);

            var memoryStream = startupMessage.SerializeFromJson<IStartupAutomation>();
            messageQueue.ReQueue(memoryStream, this._startupAutomationMsgFilename);

            this._retryCounter++;

            this._logger.Warn("Message returned to queue for retry: " + startupAutomation.Tze.Filename);
        }
    }
}