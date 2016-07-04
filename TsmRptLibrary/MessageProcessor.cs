using Revampness.Services.device.DataParser.DeviceInteractions;

namespace TsmRptLibrary
{
    internal class MessageProcessor
    {
        private IStartupAutomation _startupAutomation;

        public Patient Patient { get; set; }

        public virtual void ProcessMessageWithSerialNumber(string serialNumber)
        {
            var paceartAdapter = new PaceartAdapterWrapper();
            var Patient = new Patient(serialNumber, paceartAdapter);
            var patientGuid = Patient.PatientEnrollmentDates.PatientGuid.ToString();

            if (!Patient.ArePatientDatesofEnrollmentValid())
            {
                return;
            }

            var eventAutomationAutomator = new EventAutomationAutomator();
            eventAutomationAutomator.DeleteOldEventAutomationEntryData(patientGuid, serialNumber);
        }

        public MessageProcessor(IStartupAutomation message)
        {
            this._startupAutomation = message;
        }
    }
}