using log4net;
using System.Reflection;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class DeviceIdBySerialNumberRetrieverLogger
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void EmptySerialNumber()
        {
            this._logger.Error("This serial number is empty.");
        }

        public void DeviceIdIs(long deviceId)
        {
            this._logger.Debug("Device id is: " + deviceId);
        }

        public void SprocInvalid(string message)
        {
            this._logger.Error(message);
        }

        public void CannotGetId(string message)
        {
            this._logger.Error("Unable to retrieve device id." + message);
        }
    }
}