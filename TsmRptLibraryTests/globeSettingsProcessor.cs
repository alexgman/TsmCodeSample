using Recardo.EnterpriseServices.globe.Client;
using Recardo.EnterpriseServices.globe.Contracts;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Profusion.Services.coffee.OsdRptLibrary;
using System;
using System.Collections.Generic;

namespace OsdRptLibraryTests
{
    [TestFixture]
    public class globeSettingsProcessorTests
    {
        public List<SettingValueBinding> InitializeSettings(int timedtripsPerDay = 12, bool enableTimedEvents = true, int timedDailyFrequency = 13, int timedtripHour = 3, bool enableEctopyCounts = true)
        {
            //Arrange
            var application = ApplicationReference.ByName("Reporting");
            var area = AreaReference.ByName(application, "Default");
            var level = HierarchyLevelReference.ByName(area, "person");
            var node = HierarchyNodeReference.ByExternalKey(level, Guid.NewGuid().ToString());

            var settingTimedtripsPerDay = new SettingValueBinding
            {
                HierarchyNode = node,
                Setting = new SettingReference { Name = "TimedtripsPerDay" },
                Value = timedtripsPerDay.ToString()
            };

            var settingEnableTimedEvents = new SettingValueBinding
            {
                HierarchyNode = node,
                Setting = new SettingReference { Name = "Timed" },
                Value = enableTimedEvents.ToString()
            };

            var settingTimedDailyFrequency = new SettingValueBinding
            {
                HierarchyNode = node,
                Setting = new SettingReference { Name = "TimedDailyFrequency" },
                Value = timedDailyFrequency.ToString()
            };

            var settingTimedtripHour = new SettingValueBinding
            {
                HierarchyNode = node,
                Setting = new SettingReference { Name = "TimedtripHour" },
                Value = timedtripHour.ToString()
            };

            var settingEnableEctopyCounts = new SettingValueBinding
            {
                HierarchyNode = node,
                Setting = new SettingReference { Name = "EnableEctopyCounts" },
                Value = enableEctopyCounts.ToString()
            };

            var settingsbinding = new List<SettingValueBinding>
            {
                settingTimedtripsPerDay,
                settingEnableTimedEvents,
                settingTimedtripHour,
                settingTimedDailyFrequency,
                settingEnableEctopyCounts
            };

            return settingsbinding;
        }

        public List<SettingValueBinding> InitializeSettings()
        {
            //Arrange
            var application = ApplicationReference.ByName("Reporting");
            var area = AreaReference.ByName(application, "Default");
            var level = HierarchyLevelReference.ByName(area, "person");
            var node = HierarchyNodeReference.ByExternalKey(level, Guid.NewGuid().ToString());

            var settingTimedtripsPerDay = new SettingValueBinding
            {
                HierarchyNode = node,
                Setting = new SettingReference { Name = "TimedtripsPerDay" },
                Value = "12"
            };

            var settingEnableTimedEvents = new SettingValueBinding
            {
                HierarchyNode = node,
                Setting = new SettingReference { Name = "Timed" },
                Value = "true"
            };

            var settingTimedDailyFrequency = new SettingValueBinding
            {
                HierarchyNode = node,
                Setting = new SettingReference { Name = "TimedDailyFrequency" },
                Value = "13"
            };

            var settingTimedtripHour = new SettingValueBinding
            {
                HierarchyNode = node,
                Setting = new SettingReference { Name = "TimedtripHour" },
                Value = "3"
            };

            var settingEnableEctopyCounts = new SettingValueBinding
            {
                HierarchyNode = node,
                Setting = new SettingReference { Name = "EnableEctopyCounts" },
                Value = "true"
            };

            var settingsbinding = new List<SettingValueBinding>
            {
                settingTimedtripsPerDay,
                settingEnableTimedEvents,
                settingTimedtripHour,
                settingTimedDailyFrequency,
                settingEnableEctopyCounts
            };

            return settingsbinding;
        }

        private static readonly int[] TimedtripsPerDay_0_1 = new int[] { 0, 1 };
        private static readonly int[] TimedtripsPerDay_2_3 = new int[] { 2, 3 };
        private static readonly int[] TimedtripsPerDay_4_5 = new int[] { 4, 5 };
        private static readonly int[] TimedtripsPerDay_6_11 = new int[] { 6, 7, 8, 9, 10, 11 };
        private static readonly int[] TimedtripsPerDay_24 = new int[] { 24 };

        [Test]
        [TestCaseSource(nameof(TimedtripsPerDay_0_1))]
        public void TimedtripsPerDay_between_0_1_returns_0_1(int timedtripsArray)
        {
            var settings = new SettingValueBindingCollection(this.InitializeSettings(timedtripsPerDay: timedtripsArray));
            var sut = new globeSettingsProcessor();

            //Act
            var resultSettings = sut.GetSettings(settings);

            //Assert
            Assert.That(resultSettings.TimedtripsPerDay == timedtripsArray);
        }

        [Test]
        [TestCaseSource(nameof(TimedtripsPerDay_2_3))]
        public void TimedtripsPerDay_between_2_3_returns_2(int timedtripsArray)
        {
            var settings = new SettingValueBindingCollection(this.InitializeSettings(timedtripsPerDay: timedtripsArray));
            var sut = new globeSettingsProcessor();

            //Act
            var resultSettings = sut.GetSettings(settings);

            //Assert
            Assert.That(resultSettings.TimedtripsPerDay == 2);
        }

        [Test]
        [TestCaseSource(nameof(TimedtripsPerDay_4_5))]
        public void TimedtripsPerDay_between_4_5_returns_4(int timedtripsArray)
        {
            var settings = new SettingValueBindingCollection(this.InitializeSettings(timedtripsPerDay: timedtripsArray));
            var sut = new globeSettingsProcessor();

            //Act
            var resultSettings = sut.GetSettings(settings);

            //Assert
            Assert.That(resultSettings.TimedtripsPerDay == 4);
        }

        [Test]
        [TestCaseSource(nameof(TimedtripsPerDay_6_11))]
        public void TimedtripsPerDay_between_6_11_returns_6(int timedtripsArray)
        {
            var settings = new SettingValueBindingCollection(this.InitializeSettings(timedtripsPerDay: timedtripsArray));
            var sut = new globeSettingsProcessor();

            //Act
            var resultSettings = sut.GetSettings(settings);

            //Assert
            Assert.That(resultSettings.TimedtripsPerDay == 6);
        }

        [Test]
        [TestCaseSource(nameof(TimedtripsPerDay_24))]
        public void TimedtripsPerDay_between_24_returns_24(int timedtripsArray)
        {
            var settings = new SettingValueBindingCollection(this.InitializeSettings(timedtripsPerDay: timedtripsArray));
            var sut = new globeSettingsProcessor();

            //Act
            var resultSettings = sut.GetSettings(settings);

            //Assert
            Assert.That(resultSettings.TimedtripsPerDay == 24);
        }

        [Test]
        [TestCaseSource(nameof(TimedtripsPerDay_0_1))]
        public void when_TimedtripsPerDay_between_0_1_and_EnableTimedEvents_and_TimedtripHour_greater_than_11_then_subtract_12_from_TimedtripHour(int timedtrips)
        {
            var settings = new SettingValueBindingCollection(this.InitializeSettings(timedtripsPerDay: timedtrips, enableTimedEvents: true, timedtripHour: 12));
            var sut = new globeSettingsProcessor();

            //Act
            var resultSettings = sut.GetSettings(settings);

            //Assert
            Assert.That(resultSettings.TimedtripHour == 12);
        }

        [Test]
        [TestCaseSource(nameof(TimedtripsPerDay_2_3))]
        public void when_TimedtripsPerDay_between_2_3_and_EnableTimedEvents_and_TimedtripHour_greater_than_11_then_subtract_12_from_TimedtripHour(int timedtrips)
        {
            var settings = new SettingValueBindingCollection(this.InitializeSettings(timedtripsPerDay: timedtrips, enableTimedEvents: true, timedtripHour: 12));
            var sut = new globeSettingsProcessor();

            //Act
            var resultSettings = sut.GetSettings(settings);

            //Assert
            Assert.That(resultSettings.TimedtripHour == 0);
        }

        [Test]
        [TestCaseSource(nameof(TimedtripsPerDay_4_5))]
        public void when_TimedtripsPerDay_between_4_5_and_EnableTimedEvents_and_TimedtripHour_greater_than_5_then_subtract_12_from_TimedtripHour(int timedtrips)
        {
            var settings = new SettingValueBindingCollection(this.InitializeSettings(timedtripsPerDay: timedtrips, enableTimedEvents: true, timedtripHour: 6));
            var sut = new globeSettingsProcessor();

            //Act
            var resultSettings = sut.GetSettings(settings);

            //Assert
            Assert.That(resultSettings.TimedtripHour == 0);
        }

        [Test]
        [TestCaseSource(nameof(TimedtripsPerDay_6_11))]
        public void when_TimedtripsPerDay_between_6_11_and_EnableTimedEvents_and_TimedtripHour_greater_than_3_then_subtract_12_from_TimedtripHour(int timedtrips)
        {
            var settings = new SettingValueBindingCollection(this.InitializeSettings(timedtripsPerDay: timedtrips, enableTimedEvents: true, timedtripHour: 4));
            var sut = new globeSettingsProcessor();

            //Act
            var resultSettings = sut.GetSettings(settings);

            //Assert
            Assert.That(resultSettings.TimedtripHour == 0);
        }

        [Test]
        [TestCaseSource(nameof(TimedtripsPerDay_24))]
        public void when_TimedtripsPerDay_24_and_EnableTimedEvents_and_TimedtripHour_greater_than_11_then_subtract_12_from_TimedtripHour(int timedtrips)
        {
            var settings = new SettingValueBindingCollection(this.InitializeSettings(timedtripsPerDay: timedtrips, enableTimedEvents: true, timedtripHour: 24));
            var sut = new globeSettingsProcessor();

            //Act
            var resultSettings = sut.GetSettings(settings);

            //Assert
            Assert.That(resultSettings.TimedtripHour == 0);
        }

        [Test]
        public void TimedtripsPerDay_25_returns_12()
        {
            var settings = new SettingValueBindingCollection(this.InitializeSettings(timedtripsPerDay: 25));
            var sut = new globeSettingsProcessor();

            //Act
            var resultSettings = sut.GetSettings(settings);

            //Assert
            Assert.That(resultSettings.TimedtripsPerDay == 12);
        }

        [Test]
        public void correctly_initializes_dto_with_settings_values()
        {
            var settings = new SettingValueBindingCollection(this.InitializeSettings());
            var sut = new globeSettingsProcessor();

            //Act
            var resultSettings = sut.GetSettings(settings);

            //Assert
            Assert.That(resultSettings.TimedtripsPerDay == 12);
            Assert.That(resultSettings.EnableTimedEvents);
            Assert.That(resultSettings.TimedDailyFrequency == 13);
            Assert.That(resultSettings.TimedtripHour == 1);
            Assert.That(resultSettings.EnableEctopyCounts);
        }
    }
}