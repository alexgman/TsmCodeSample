using log4net;
using System.Reflection;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class AutomationFeederLogger
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void UnknownTableStand()
        {
            this._logger.Error("The service mode for this person is unknown. Message will be returend.");
        }

        public void NoEntries()
        {
            this._logger.Error("No event automation entries will be created.");
        }

        public void FeedException(string message)
        {
            this._logger.Error(message);
        }
    }
}