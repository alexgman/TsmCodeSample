namespace TsmRptLibrary
{
    internal static class AtlasSettingsReaderExtensions
    {
        public static AtlasSettingsDto UpdateTimedStrips(this AtlasSettingsDto atlasSettingsDto)
        {
            if (atlasSettingsDto.TimedStripsPerDay.IsBetween(0, 1))
            {
                //Intentional
            }
            else if (atlasSettingsDto.TimedStripsPerDay.IsBetween(2, 3))
            {
                atlasSettingsDto.TimedStripsPerDay = 2;

                if (atlasSettingsDto.EnableTimedEvents && (atlasSettingsDto.TimedStripHour > 11))
                {
                    atlasSettingsDto.TimedStripHour -= 12;
                }
            }
            else if (atlasSettingsDto.TimedStripsPerDay.IsBetween(4, 5))
            {
                if (atlasSettingsDto.EnableTimedEvents && (atlasSettingsDto.TimedStripHour > 5))
                {
                    atlasSettingsDto.TimedStripHour %= 6;
                }
                atlasSettingsDto.TimedStripsPerDay = 4;
            }
            else if (atlasSettingsDto.TimedStripsPerDay.IsBetween(6, 11))
            {
                if (atlasSettingsDto.EnableTimedEvents && (atlasSettingsDto.TimedStripHour > 3))
                {
                    atlasSettingsDto.TimedStripHour %= 4;
                }
                atlasSettingsDto.TimedStripsPerDay = 6;
            }
            else if (atlasSettingsDto.TimedStripsPerDay == 24)
            {
                if (atlasSettingsDto.EnableTimedEvents && (atlasSettingsDto.TimedStripHour > 0))
                {
                    atlasSettingsDto.TimedStripHour = 0;
                }
            }
            else
            {
                atlasSettingsDto.TimedStripsPerDay = 12;
                if (atlasSettingsDto.EnableTimedEvents && (atlasSettingsDto.TimedStripHour > 1))
                {
                    //If even start at 0, else start on 1am.
                    atlasSettingsDto.TimedStripHour %= 2;
                }
            }

            return atlasSettingsDto;
        }
    }
}