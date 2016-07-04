using log4net;
using System.Reflection;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class EventEntryMetaDataCreatorLogger
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Created(EventEntryMetaDataDto entryMetaData)
        {
            this._logger.Info("Process Name: " + entryMetaData.ProcessName);
            this._logger.Info("Automation DateTime: " + entryMetaData.AutomationDateTime);
            this._logger.Info("Event Automation Id: " + entryMetaData.yawnwrappingId);
        }
    }
}