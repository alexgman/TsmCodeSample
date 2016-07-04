namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class globeSettingsDto
    {
        private const int TimedDailyFrequencyDefault = 1;
        private const int TimedtripHourDefault = 6;
        private const int TimedtripsPerDayDefault = 1;

        public bool EnableEctopyCounts { get; set; }

        public bool EnableTimedEvents { get; set; }

        public bool EnableViaFullDisclosure { get; set; }

        public bool EnableViaReport { get; set; }

        public int TimedDailyFrequency { get; set; } = TimedDailyFrequencyDefault;

        public int TimedtripHour { get; set; } = TimedtripHourDefault;

        public int TimedtripsPerDay { get; set; } = TimedtripsPerDayDefault;
    }
}