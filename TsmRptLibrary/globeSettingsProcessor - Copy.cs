using Recardo.EnterpriseServices.globe.Contracts;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class globeSettingsProcessor : IglobeSettingsProcessor
    {
        private readonly globeSettingsProcessorLogger _logger = new globeSettingsProcessorLogger();

        public globeSettingsDto GetSettings(SettingValueBindingCollection items)
        {
            var globeSettingsDto = this.InitializeglobeSettingsDto(items);

            this._logger.Read(globeSettingsDto, "Settings before updating timed trips:");
            var updatedglobeSettings = this.UpdateTimedtrips(globeSettingsDto);
            this._logger.Read(updatedglobeSettings, "Settings after updating timed trips:");

            return updatedglobeSettings;
        }

        private globeSettingsDto InitializeglobeSettingsDto(SettingValueBindingCollection items)
        {
            var globeSettingsDto = new globeSettingsDto();
            globeSettingsDto.EnableTimedEvents = items["Timed"].IfBoolThenParseElseDefault(false);
            globeSettingsDto.TimedDailyFrequency = items["TimedDailyFrequency"].IfIntThenParseElseDefault(1);
            globeSettingsDto.TimedtripsPerDay = items["TimedtripsPerDay"].IfIntThenParseElseDefault(1);
            globeSettingsDto.TimedtripHour = items["TimedtripHour"].GetHours(6);
            globeSettingsDto.EnableEctopyCounts = items["EnableEctopyCounts"].IfBoolThenParseElseDefault(false);
            return globeSettingsDto;
        }

        private globeSettingsDto UpdateTimedtrips(globeSettingsDto globeSettingsDto)
        {
            if (globeSettingsDto.TimedtripsPerDay.IsBetween(0, 1))
            {
                //Intentional
            }
            else if (globeSettingsDto.TimedtripsPerDay.IsBetween(2, 3))
            {
                globeSettingsDto.TimedtripsPerDay = 2;

                if (globeSettingsDto.EnableTimedEvents && (globeSettingsDto.TimedtripHour > 11))
                {
                    globeSettingsDto.TimedtripHour -= 12;
                }
            }
            else if (globeSettingsDto.TimedtripsPerDay.IsBetween(4, 5))
            {
                if (globeSettingsDto.EnableTimedEvents && (globeSettingsDto.TimedtripHour > 5))
                {
                    globeSettingsDto.TimedtripHour %= 6;
                }
                globeSettingsDto.TimedtripsPerDay = 4;
            }
            else if (globeSettingsDto.TimedtripsPerDay.IsBetween(6, 11))
            {
                if (globeSettingsDto.EnableTimedEvents && (globeSettingsDto.TimedtripHour > 3))
                {
                    globeSettingsDto.TimedtripHour %= 4;
                }
                globeSettingsDto.TimedtripsPerDay = 6;
            }
            else if (globeSettingsDto.TimedtripsPerDay == 24)
            {
                if (globeSettingsDto.EnableTimedEvents && (globeSettingsDto.TimedtripHour > 0))
                {
                    globeSettingsDto.TimedtripHour = 0;
                }
            }
            else
            {
                globeSettingsDto.TimedtripsPerDay = 12;
                if (globeSettingsDto.EnableTimedEvents && (globeSettingsDto.TimedtripHour > 1))
                {
                    //If even start at 0, else start on 1am.
                    globeSettingsDto.TimedtripHour %= 2;
                }
            }

            return globeSettingsDto;
        }
    }
}