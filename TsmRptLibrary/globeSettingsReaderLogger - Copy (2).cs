using Recardo.EnterpriseServices.globe.Contracts;
using log4net;
using System.Reflection;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class globeSettingsReaderLogger
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void globeSettingsForperson(SettingValueBindingCollection settings)
        {
            foreach (var item in settings)
            {
                this._logger.Info(item.Setting + " = " + item.Value);
            }
        }

        public void UnableToConnect()
        {
            this._logger.Error("Unable to connect to globe to get settings. Make sure the username that is executing is able to authenticate.");
        }
    }
}