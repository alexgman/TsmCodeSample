namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal interface IglobeSettingsDto
    {
        bool EnableEctopyCounts { get; set; }

        bool EnableTimedEvents { get; set; }

        bool EnableViaFullDisclosure { get; set; }

        bool EnableViaReport { get; set; }

        int TimedDailyFrequency { get; set; }

        int TimedtripHour { get; set; }

        int TimedtripsPerDay { get; set; }
    }
}