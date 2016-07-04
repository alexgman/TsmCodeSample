using System;

namespace TsmRptLibrary
{
    internal class AtlasConfigurationItems7 : IAtlasConfigurationItems
    {
        private const int TimedDailyFrequencyDefault = 1;
        private const int TimedStripHourDefault = 6;
        private const int TimedStripsPerDayDefault = 1;

        public void FetchAtlasSettings()
        {
            throw new NotImplementedException();
        }

        public int CalculateTimedStripsHourIncrementor
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public AtlasConfigurationItemsWrapper.ServiceMode CurrentServiceMode
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool EnableEctopyCounts { get; set; }

        public bool EnableTimedEvents { get; set; }

        public bool EnableViaFullDisclosure { get; set; }

        public bool EnableViaReport { get; set; }

        public int TimedDailyFrequency { get; set; } = TimedDailyFrequencyDefault;

        public int TimedStripHour { get; set; } = TimedStripHourDefault;

        public int TimedStripsPerDay { get; set; } = TimedStripsPerDayDefault;

        public bool AreTimedEventsEnabled(int dayCounter)
        {
            throw new NotImplementedException();
        }

        public void UpdatePropertiesBasedOnAtlasSettings()
        {
            throw new NotImplementedException();
        }

        public void UpdateTimedStrips()
        {
            throw new NotImplementedException();
        }

        public void UpdateWithAtlasData()
        {
            throw new NotImplementedException();
        }
    }
}