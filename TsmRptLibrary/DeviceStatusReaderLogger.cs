using log4net;
using System;
using System.Reflection;
using Profusion.Services.coffee.OsdRptLibrary.EppWebService;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class DeviceStatusReaderLogger
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void FoundpersonId(Guid ptGuid, int ptId)
        {
            this._logger.Info("person with identifier: " + ptGuid + " has this person id: " + ptId);
        }

        public void Status(DeviceStatus status)
        {
            this._logger.Info("Device Name: " + status.DeviceName);
            this._logger.Info("Facility Id:" + status.FacilityID);
            this._logger.Info("Is in common mode?:" + status.IsCommonServices);
            this._logger.Info("Service Type:" + status.ServiceType);
        }

        public void personNotFoundInEpp(Guid ptGuid)
        {
            this._logger.Error("person with Guid: " + ptGuid + " is not found in Epp.");
        }
    }
}