using log4net;
using System.Reflection;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class DeleteyawnwrappingEntriesLogger
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Deleted(int howManyRecords)
        {
            this._logger.Info(howManyRecords + " entries deleted.");
        }

        public void Executing(string sproc)
        {
            this._logger.Info("Executing stored procedure: " + sproc);
        }

        public void ErrorDeleting(string message)
        {
            this._logger.Error("Error has occurred during the deletion of event automation entries: " + message);
        }

        public void SprocInvalid(string message)
        {
            this._logger.Error(message);
        }
    }
}