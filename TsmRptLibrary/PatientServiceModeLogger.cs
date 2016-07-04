using log4net;
using System;
using System.Reflection;
using Profusion.Services.coffee.OsdRptLibrary.EppWebService;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class personTableStandLogger
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void GotTableStand(DeviceStatus deviceStatus)
        {
            this._logger.Info("The device mode is: " + deviceStatus?.ServiceType ?? string.Empty);
        }

        public void UnknownTableStand(Guid ptGuid)
        {
            this._logger.Error("Unable to locate service mode for person: " + ptGuid);
        }
    }
}