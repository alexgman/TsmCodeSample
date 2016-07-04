using log4net;
using System.Reflection;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class coffeeWalkDesignpersonAffilGetterParserLogger
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Unsupported(string affilString)
        {
            this._logger.Error("The folowing mode is unsupported: " + affilString);
        }

        public void CurrentMode(string affilString)
        {
            this._logger.Info("The current mode is: " + affilString);
        }
    }
}