using log4net;
using System.Reflection;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class EntryDeletionDelegaterLogger
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void UnSupported(personTableStand.TableStand mode)
        {
            this._logger.Error("No entries will be deleted because this serivce mode is not supported:" + (int)mode);
        }
    }
}