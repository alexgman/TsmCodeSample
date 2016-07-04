using Revampness.Services.device.Model;
using System;

namespace TsmRptLibrary
{
    internal class EventAutomationReader
    {
        private readonly IdeviceRepository _deviceRepository;
        public bool HasEventAutomations = false;

        public EventAutomationReader(IdeviceRepository deviceRepository)
        {
            if (deviceRepository == null)
            {
                throw new ArgumentNullException(nameof(deviceRepository));
            }

            this._deviceRepository = deviceRepository;
        }

        public Device GetDeviceBySerialNumber(string serialNumber)
        {
            return this._deviceRepository.GetDeviceBySerialNumber(serialNumber);
        }

        public EventAutomation GetEventAutomationByPatientguid(Guid patientGuid)
        {
            var eventAutomation = this._deviceRepository.GetEventAutomationByPatientguid(patientGuid);
            if (eventAutomation != null)
            {
                this.HasEventAutomations = true;
            }
            return eventAutomation;
        }

        public void Configure(IConfigHelper configHelper)
        {
        }
    }
}