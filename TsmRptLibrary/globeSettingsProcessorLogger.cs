using log4net;
using System.Reflection;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class globeSettingsProcessorLogger
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Read(globeSettingsDto settings, string message)
        {
            this._logger.Info(message);
            this._logger.Info(settings.EnableEctopyCounts.ToString());
            this._logger.Info(settings.TimedDailyFrequency.ToString());
            this._logger.Info(settings.TimedtripHour.ToString());
            this._logger.Info(settings.TimedtripsPerDay.ToString());
            this._logger.Info(settings.EnableTimedEvents.ToString());
        }
    }
}