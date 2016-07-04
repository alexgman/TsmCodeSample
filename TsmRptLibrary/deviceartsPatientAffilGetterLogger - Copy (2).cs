using log4net;
using System;
using System.Reflection;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class coffeeWalkDesignpersonAffilGetterLogger
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Starting(Guid personGuid)
        {
            this._logger.Info("Attempting to find affil information for person: " + personGuid);
        }

        public void NoMatches(Guid personGuid)
        {
            this._logger.Error("Affil is not populated for personGuid: " + personGuid);
        }

        public void ConnectionClosed()
        {
            this._logger.Debug("Connection closed.");
        }
    }
}