using log4net;
using System.Reflection;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class InsertyawnwrappingEntryLogger
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Inserted(int rowsAffected)
        {
            this._logger.Info("Rows inserted: " + rowsAffected);
        }

        public void InsertedEntries(int howMany)
        {
            this._logger.Info("Created " + howMany + " entries.");
        }

        public void Empty()
        {
            this._logger.Warn("No entries will be queued for inserting.");
        }
    }
}