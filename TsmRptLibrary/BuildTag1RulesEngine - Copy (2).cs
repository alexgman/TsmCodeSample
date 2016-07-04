using System;
using System.Reflection;
using System.Threading;

namespace TsmRptLibrary
{
    internal class BuildTag1RulesEngine : IBuildTag1RulesEngine
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected ILog GetLogger()
        {
            return LogManager.GetLogger(this.GetType().Assembly, this.GetType().Name + '.' + Thread.CurrentThread.ManagedThreadId);
        }

        private readonly IPatientServiceMode _patientServiceMode;
        private readonly IAtlasSettingsReader _atlasSettingsReader;
        private readonly IPatientEnrollmentDatesValidator _patientEnrollmentDatesValidator;

        public BuildTag1RulesEngine(IPatientServiceMode patientServiceMode, IAtlasSettingsReader atlasSettingsReader, IPatientEnrollmentDatesValidator patientEnrollmentDatesValidator)
        {
            this._patientServiceMode = patientServiceMode;
            this._atlasSettingsReader = atlasSettingsReader;
            this._patientEnrollmentDatesValidator = patientEnrollmentDatesValidator;
        }

        public Tag1RulesEngine BuildRulesEngine(Guid patientGuid, string serialNumber, IdeviceRepository deviceRepository)
        {
            var device = deviceRepository.GetDeviceBySerialNumber(serialNumber);
            var ea = deviceRepository.GetEventAutomationByPatientguid(patientGuid);
            var serviceMode = this._patientServiceMode.GetServiceMode(new DeviceStatusReader(new PhysicianPortalServiceContractClient(),
                new TaskCreatorAndWorkerServiceClient()), patientGuid);
            var atlasResult = this._atlasSettingsReader.GetSettingsForPatient(patientGuid);
            //TODO: dont force multiple calls to getsettinugsforpatient
            return new Tag1RulesEngine
                (new Tag1Dto
                {
                    AtlasEnabledTimedEvents = atlasResult.EnableTimedEvents,
                    AtlasTimedDailyFrequency = atlasResult.TimedDailyFrequency,
                    AtlasTimedStripHour = atlasResult.TimedStripHour,
                    AtlasTimedStripsPerDay = atlasResult.TimedStripsPerDay,
                    DeviceId = device.Id,
                    PatientEnrollmentStart = this._patientEnrollmentDatesValidator.PatientEnrollmentDates.StartDate,
                    PatientEnrollmentEnd = this._patientEnrollmentDatesValidator.PatientEnrollmentDates.EndDate,
                    PatientGuid = patientGuid,
                    PatientServiceMode = serviceMode,
                    AtlasEnabledEctopyCounts = atlasResult.EnableEctopyCounts,
                    DayCounter = 0,
                    SerialNumber = serialNumber,
                    EventAutomation = ea
                });
        }
    }
}