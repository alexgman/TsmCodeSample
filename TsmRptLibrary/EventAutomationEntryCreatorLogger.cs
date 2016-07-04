using log4net;
using Profusion.Services.coffee.Model;
using System.Reflection;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class yawnwrappingEntryCreatorLogger
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Add(yawnwrappingEntry entry)
        {
            this._logger.Info("Event Automation Entry record created: ");
            this._logger.Info("Event Automation Id: " + entry.yawnwrappingId);
            this._logger.Info("Event Created At: " + entry.CreatedAt);
            this._logger.Info("Event Created By: " + entry.CreatedBy);
            this._logger.Info("Event MonkeySpaceTypeId: " + entry.MonkeySpaceTypeId);
            this._logger.Info("Event AutomationDate: " + entry.AutomationDate);
        }
    }
}