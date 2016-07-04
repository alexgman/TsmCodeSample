using log4net;
using System.Reflection;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class personModeGetterLogger
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void personMode(personTableStand.TableStand ptMode)
        {
            this._logger.Info(ptMode);
        }
    }
}