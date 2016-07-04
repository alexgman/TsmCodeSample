namespace TsmRptLibrary
{
    internal interface IAtlasConfigurationItems
    {
        bool AreTimedEventsEnabled(int dayCounter);

        bool EnableEctopyCounts { get; set; }

        bool EnableTimedEvents { get; set; }

        bool EnableViaFullDisclosure { get; set; }

        bool EnableViaReport { get; set; }

        int TimedDailyFrequency { get; set; }

        int TimedStripHour { get; set; }

        int TimedStripsPerDay { get; set; }

        int CalculateTimedStripsHourIncrementor { get; }

        //PatientServiceMode.ServiceMode CurrentServiceMode { get; }

        void UpdatePropertiesBasedOnAtlasSettings();

        void UpdateTimedStrips();

        void FetchAtlasSettings();
    }
}