using eCardio.EnterpriseServices.Atlas.Contracts;
using System;

namespace Revampness.Services.device.ReportTablePreprocessor
{
    public class AtlasConfigurationItemsWrapper : IAtlasConfigurationItems
    {
        private const int TimedDailyFrequencyDefault = 1;
        private const int TimedStripHourDefault = 6;
        private const int TimedStripsPerDayDefault = 1;
        private readonly IAtlasClient _atlasClient;
        private readonly IConfigHelper _configHelper;
        private SettingValueBindingCollection _atlasItems;

        private PatientServiceMode.ServiceMode _currentServiceMode = PatientServiceMode.ServiceMode.Unknown;
        private Guid _patientGuid;

        public AtlasConfigurationItemsWrapper(IConfigHelper configHelper, IAtlasClient atlasClient, Guid patientGuid,
            SettingValueBindingCollection atlasItems = null)
        {
            this._configHelper = configHelper;
            this._atlasClient = atlasClient;
            this._patientGuid = patientGuid;
            this._atlasItems = atlasItems;
        }

        public int CalculateTimedStripsHourIncrementor
        {
            get
            {
                return this.TimedStripsPerDay == 1
                    ? 0
                    : this.TimedStripsPerDay == 3
                        ? 12
                        : this.TimedStripsPerDay == 5 ? 6 : this.TimedStripsPerDay == 11 ? 4 : this.TimedStripsPerDay == 24 ? 1 : 2;
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
            return this.EnableTimedEvents && (dayCounter % this.TimedDailyFrequency == 0);
        }

        public void FetchAtlasSettings()
        {
            if (this._configHelper.IsEmailConfigured)
            {
                this._atlasClient.SetWindowsCredentials(this._configHelper.DomainUserName, this._configHelper.DomainPassword,
                    this._configHelper.Domain);
            }

            var application = ApplicationReference.ByName("Reporting");
            var area = AreaReference.ByName(application, "Default");
            var level = HierarchyLevelReference.ByName(area, "Patient");
            var node = HierarchyNodeReference.ByExternalKey(level, this._patientGuid.ToString());

            this._atlasItems = this._atlasClient.GetCurrentValuesNonStatic(node);
        }

        public void UpdatePropertiesBasedOnAtlasSettings(AtlasSettingsDto atlasSettingsDto)
        {
            atlasSettingsDto.EnableTimedEvents = this._atlasItems["Timed"].IfBoolThenParseElseDefault(false);
            atlasSettingsDto.TimedDailyFrequency = this._atlasItems["TimedDailyFrequency"].IfIntThenParseElseDefault(1);
            atlasSettingsDto.TimedStripsPerDay = this._atlasItems["TimedStripsPerDay"].IfIntThenParseElseDefault(1);
            atlasSettingsDto.TimedStripHour = this._atlasItems["TimedStripHour"].GetHours(6);
            atlasSettingsDto.EnableViaReport = this._atlasItems["EnableViaReport"].IfBoolThenParseElseDefault(false);
            atlasSettingsDto.EnableViaFullDisclosure = this._atlasItems["EnableViaFullDisclosure"].IfBoolThenParseElseDefault(false);
            atlasSettingsDto.EnableEctopyCounts = this._atlasItems["EnableEctopyCounts"].IfBoolThenParseElseDefault(false);
        }

        public void UpdateTimedStrips()
        {
            if (this.TimedStripsPerDay.IsBetween(0, 1))
            {
                //Intentional
            }
            else if (this.TimedStripsPerDay.IsBetween(2, 3))
            {
                this.TimedStripsPerDay = 2;

                if (this.EnableTimedEvents && (this.TimedStripHour > 11))
                {
                    this.TimedStripHour -= 12;
                }
            }
            else if (this.TimedStripsPerDay.IsBetween(4, 5))
            {
                if (this.EnableTimedEvents && (this.TimedStripHour > 5))
                {
                    this.TimedStripHour %= 6;
                }
                this.TimedStripsPerDay = 4;
            }
            else if (this.TimedStripsPerDay.IsBetween(6, 11))
            {
                if (this.EnableTimedEvents && (this.TimedStripHour > 3))
                {
                    this.TimedStripHour %= 4;
                }
                this.TimedStripsPerDay = 6;
            }
            else if (this.TimedStripsPerDay == 24)
            {
                if (this.EnableTimedEvents && (this.TimedStripHour > 0))
                {
                    this.TimedStripHour = 0;
                }
            }
            else
            {
                this.TimedStripsPerDay = 12;
                if (this.EnableTimedEvents && (this.TimedStripHour > 1))
                {
                    //If even start at 0, else start on 1am.
                    this.TimedStripHour %= 2;
                }
            }
        }
    }
}