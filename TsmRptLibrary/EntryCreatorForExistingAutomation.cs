using System;
using System.Configuration;

namespace TsmRptLibrary
{
    internal class EntryCreatorForExistingAutomation
    {
        private TsmRptProcessor _tsmRptProcessor;

        public EntryCreatorForExistingAutomation(TsmRptProcessor tsmRptProcessor)
        {
            this._tsmRptProcessor = tsmRptProcessor;
        }

        public void Init()
        {
            this._tsmRptProcessor._deviceConnectionString = ConfigurationManager.ConnectionStrings["device"].ConnectionString;
            this._tsmRptProcessor._eventAutomationIdGetter = new EventAutomationIdGetter();
            this._tsmRptProcessor._deleteEventAutomationEntries = new DeleteEventAutomationEntries(this._tsmRptProcessor._deviceConnectionString);
            this._tsmRptProcessor._deviceRepository = new deviceRepositoryWrapper();
            this._tsmRptProcessor._deviceUpdater = new DeviceUpdater();
            this._tsmRptProcessor._paceartAdapter = new PaceartAdapterWrapper();
            this._tsmRptProcessor._paceartEnrollmentDatesReader = new PaceartEnrollmentDatesReader(this._tsmRptProcessor._paceartAdapter);
            this._tsmRptProcessor._entryCollectionBuilder = new EntryCollectionBuilder();
            this._tsmRptProcessor._insertEventAutomationEntry = new InsertEventAutomationEntry(this._tsmRptProcessor._deviceConnectionString);
            this._tsmRptProcessor._enrollmentDatesValidator = new EnrollmentDatesValidator();
            this._tsmRptProcessor._patientModeGetter = new PatientModeGetter();
            this._tsmRptProcessor._enrollmentConvertedMessageParser = new EnrollmentConvertedMessageParser();
        }

        public int Process(Guid ptGuid, string serialNumber)
        {
            this.Init();

            //Get related event automation id
            var eventAutomationId = this._tsmRptProcessor._eventAutomationIdGetter.GetEventAutomationId(this._tsmRptProcessor._deviceConnectionString, ptGuid, serialNumber);

            if (eventAutomationId == 0)
            {
                return 1;
            }

            //Mct or Cem?
            //var mode = this._patientServiceMode.GetServiceMode(this._deviceStatusReader, ptGuid);
            var mode = this._tsmRptProcessor._patientModeGetter.GetPatientMode(ptGuid);

            //Delete existing child entries based on patient service mode
            if (mode == PatientServiceMode.ServiceMode.Cem)
            {
                this._tsmRptProcessor._deleteEventAutomationEntries.DeleteAllNonTimedEntries(serialNumber, ptGuid);
            }
            else
            {
                if (mode != PatientServiceMode.ServiceMode.Unknown)
                {
                    this._tsmRptProcessor._deleteEventAutomationEntries.DeleteAllChildEntries(eventAutomationId);
                }
            }

            //Get event automation record

            var eventAutomationBound = this._tsmRptProcessor._deviceRepository.GetEventAutomationByPatientGuid(ptGuid);

            if (eventAutomationBound == null)
            {
                this._tsmRptProcessor._logger.UnableToRetrieveEventAutomation();
                return 1;
            }

            //Update device id if patient switched devices
            this._tsmRptProcessor._deviceUpdater.UpdateDeviceId(eventAutomationBound, serialNumber, this._tsmRptProcessor._deviceRepository);

            //Retrieve paceart enrollment data
            var ptEnrollmentDates = this._tsmRptProcessor._paceartEnrollmentDatesReader.GetPatientEnrollmentDates(serialNumber);
            this._tsmRptProcessor._enrollmentDatesValidator.Configure(ptEnrollmentDates);
            if (!this._tsmRptProcessor._enrollmentDatesValidator.IsEnrollmentValid(eventAutomationBound.EndDate))
            {
                this._tsmRptProcessor._logger.InvalidEnrollment();
                return 1;
            }

            //Create automation entries
            var automationEntries = this._tsmRptProcessor._entryCollectionBuilder.PopulateCollectionOfEntries(eventAutomationBound.EndDate, ptEnrollmentDates,
                eventAutomationId, ptGuid, mode);
            this._tsmRptProcessor._insertEventAutomationEntry.InsertChildEntries(automationEntries);

            return 0;
        }
    }
}