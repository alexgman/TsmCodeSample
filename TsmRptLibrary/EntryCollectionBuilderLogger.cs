using System.Reflection;
using log4net;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class EntryCollectionBuilderLogger
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void NoEntriesRequired()
        {
            this._logger.Info("Based on the Derailment and globe settings for this Derailment, there will not be any data request entries created.");
        }
    }
}