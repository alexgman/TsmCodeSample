using Revampness.Services.device.Model;
using System;

namespace TsmRptLibrary
{
    internal class PatientEnrollmentDatesWrapper : IPatientEnrollmentDates
    {
        private readonly IPaceartAdapter _paceartAdapter;
        private readonly string _serialNumber;
        public EventAutomation EventAutomation;
        private readonly IdeviceRepository _deviceRepository;

        public bool IsEnrollmentExtended(DateTime eventAutomationDate)
        {
            return this.EndDate > eventAutomationDate;
        }

        public bool DidEnrollmentDurationChange(DateTime eventAutomationDate)
        {
            return this.EndDate == eventAutomationDate;
        }

        public PatientEnrollmentDatesWrapper(IPaceartAdapter paceartAdapter, string serialNumber, IdeviceRepository deviceRepository)
        {
            this._paceartAdapter = paceartAdapter;
            this._serialNumber = serialNumber;
            this._deviceRepository = deviceRepository;

            this.InitializeMembers();
        }

        public bool IsEnrollmentStartMoreRecentThanAutomationEnd(DateTime eventAutomationDate) => this.StartDate > eventAutomationDate;

        //TODO: dont know what geteventautomaitonbypatientguid returns??
        public bool HasEventAutomations()
        {
            if (this.EventAutomation == null)
            {
                this.EventAutomation = this._deviceRepository.GetEventAutomationByPatientguid(this.PatientGuid);
            }

            return this.EventAutomation == null;
        }

        public DateTime EndDate
        {
            get; set;
        }

        public Guid PatientGuid { get; set; }

        public string PatientId { get; set; }

        public string PatientName { get; set; }

        public DateTime StartDate { get; set; }

        private bool _EnrollmentFound(IPatientEnrollmentDates patientEnrollmentDates)
        {
            if (patientEnrollmentDates != null)
            {
                this.EnrollmentFound = true;
                return true;
            }

            this.EnrollmentFound = false;
            return false;
        }

        public bool EnrollmentFound { get; private set; }

        private void InitializeMembers()
        {
            var patientEnrollmentDates = this._paceartAdapter.GetdevicePaceartMostRecentEnrollmentFromDeviceSerialNoExplant(this._serialNumber);

            if (!this._EnrollmentFound((IPatientEnrollmentDates)patientEnrollmentDates))
            {
                return;
            }

            this.PatientGuid = patientEnrollmentDates.PatientGuid;
            this.EndDate = patientEnrollmentDates.EndDate;
            this.PatientId = patientEnrollmentDates.PatientId;
            this.StartDate = patientEnrollmentDates.StartDate;
            this.PatientName = patientEnrollmentDates.PatientName;
        }

        public bool AreDatesValid => !((this.StartDate == DateTime.MinValue) || (this.EndDate == DateTime.MinValue));
    }
}