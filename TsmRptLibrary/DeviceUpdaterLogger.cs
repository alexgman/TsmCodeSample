using log4net;
using System.Reflection;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class DeviceUpdaterLogger
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void DeviceUpdated(long oldId, long newId)
        {
            this._logger.Info("The old device id was: " + oldId);
            this._logger.Info("The new device id is: " + newId);
        }
    }
}