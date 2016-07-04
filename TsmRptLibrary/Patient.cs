using System;

namespace TsmRptLibrary
{
    internal class Patient
    {
        public EventAutomation EventAutomation;
        private readonly IConfigHelper _configHelper;
        private readonly IDeviceStatus _patientDeviceStatus = null;
        private readonly IPatientEnrollmentDates _patientEnrollmentDates = null;
        private readonly string _serialNumber;
        private readonly IdeviceRepository _deviceRepository;

        public void SetDeviceIdFromdevice() => this.EventAutomation.DeviceId = this._deviceRepository.GetDeviceIdForSerialNumber(this._serialNumber);

        public Patient(string serialNumber, IConfigHelper configHelper, IdeviceRepository deviceRepository)
        {
            this._serialNumber = serialNumber;
            this._configHelper = configHelper;
            this._deviceRepository = deviceRepository;
        }

        public bool AreDatesOfEnrollementValid => !this.GetPatientEnrollmentDates.StartDate.IsMin() && !this.GetPatientEnrollmentDates.EndDate.IsMin();

        public IDeviceStatus GetPatientDeviceStatus => this._patientDeviceStatus ?? this.InitializePatientDeviceStatusFromEpp();

        public IPatientEnrollmentDates GetPatientEnrollmentDates => this._patientEnrollmentDates ?? this.InitializePatientEnrollmentDates(new PaceartAdapterWrapper());

        private PhysicianPortalServiceContract GetService => new PhysicianPortalClient();

        private int PatientGuidToId => Convert.ToInt32(DiService.PatientGuidToPatientId(this.GetPatientEnrollmentDates.PatientGuid));

        public SettingValueBindingCollection GetAtlasItems(IAtlasClient atlasClient)
        {
            if (this._configHelper.IsEmailConfigured)
            {
                atlasClient.SetWindowsCredentials(this._configHelper.DomainUserName, this._configHelper.DomainPassword, this._configHelper.Domain);
            }

            var patientGuid = this._patientEnrollmentDates.PatientGuid;
            var application = ApplicationReference.ByName("Reporting");
            var area = AreaReference.ByName(application, "Default");
            var level = HierarchyLevelReference.ByName(area, "Patient");
            var node = HierarchyNodeReference.ByExternalKey(level, patientGuid.ToString());

            return atlasClient.GetCurrentValues(node);
        }

        public bool HasEventAutomations(Guid patientGuid, IdeviceRepository deviceRepository)
        {
            if (this.EventAutomation == null)
            {
                this.EventAutomation = deviceRepository.GetEventAutomationByPatientguid(patientGuid);
            }

            return this.EventAutomation.PatientGuid != Guid.Empty;
        }

        public bool DidEnrollmentEndAfterAutomation => this.GetPatientEnrollmentDates.EndDate > this.EventAutomation.EndDate;

        private IDeviceStatus InitializePatientDeviceStatusFromEpp() => this.GetService.GetPatientDeviceStatus(this.PatientGuidToId);

        private IPatientEnrollmentDates InitializePatientEnrollmentDates(IPaceartAdapter paceartAdapter) => paceartAdapter.GetdevicePaceartMostRecentEnrollmentFromDeviceSerialNoExplant(this._serialNumber);
    }
}