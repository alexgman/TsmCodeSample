namespace TsmRptLibrary
{
    internal class EventAutomationWriter
    {
        private IdeviceRepository deviceRepository;

        public EventAutomationWriter(IdeviceRepository deviceRepository)
        {
            this.deviceRepository = deviceRepository;
        }
    }
}