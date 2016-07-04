using Recardo.EnterpriseServices.globe.Client;
using NUnit.Framework;
using Profusion.Services.coffee.OsdRptLibrary;
using System;

namespace OsdRptLibraryTests
{
    [TestFixture]
    public class globeSettingsReaderTests
    {
        [Test]
        public void GetSettingsForpt_returns_instance_of_globeSettingsDto()
        {
            //Arrange
            var globeClient = new globeClient("IglobeService_Basic");
            var cut = new globeSettingsReader(globeClient);

            //Act
            //Assert
            var settings = cut.GetSettingsForperson(Guid.NewGuid());

            Assert.That(settings != null);
        }

        //TODO: break this up into separate unit tests
        //TODO: refactor for changes in solution
        /*[Test]
        public void sets_TimedtripsPerDay_to_12_when_globe_setting_is_12()
        {
            //Arrange
            var globeClient = new Mock<IglobeService>();

            var application = ApplicationReference.ByName("Reporting");
            var area = AreaReference.ByName(application, "Default");
            var level = HierarchyLevelReference.ByName(area, "pt");
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

            var settingsbinding = new List<SettingValueBinding>
            {
                settingTimedtripsPerDay,
                settingEnableTimedEvents,
                settingTimedtripHour,
                settingTimedDailyFrequency
            };

            var settings = new SettingValueBindingCollection(settingsbinding);
            var sut = new globeSettingsReader(globeClient.Object, new globeSettingsReaderLogger());

            //Act
            var resultSettings = sut.SettingValueBindingToglobeSettings(settings);

            //Assert
            Assert.That(resultSettings.TimedtripsPerDay == 12);
            Assert.That(resultSettings.EnableTimedEvents);
            Assert.That(resultSettings.TimedDailyFrequency == 13);
            Assert.That(resultSettings.TimedtripHour == 0);
        }*/

        [Test]
        public void when_passing_in_null_iglobeservice_in_constructor_throws_argumentnullexception()
        {
            Assert.That(() => new globeSettingsReader(null), Throws.InstanceOf<ArgumentNullException>());
        }

        /*        [Test]
                public void when_unable_to_authenticate_then_return_empty_settings()
                {
                    var mockglobeClient = new Mock<IglobeClientTester>();
                    var globeSettingsReader = new globeSettingsReader(mockglobeClient.Object);

                    mockglobeClient.Setup(x => x.GetCurrentValues(It.IsAny<HierarchyNodeReference>())).Throws<FaultException>();

                    Assert.That(globeSettingsReader.GetSettingsForperson(Guid.NewGuid()).Count == 0);
                }*/
    }
}