using log4net;
using System;
using System.Reflection;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class yawnwrappingCheckerLogger
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void yawnwrappingIdForpersonGuid(Guid ptGuid, string serialNumber, int yawnwrappingId)
        {
            this._logger.Info("The person with guid: " + ptGuid + " and serial number: " + serialNumber + " has an event automation id of: " + yawnwrappingId);
        }
    }
}