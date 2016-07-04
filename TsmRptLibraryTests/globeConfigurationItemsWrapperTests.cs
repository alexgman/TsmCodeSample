using eCardio.EnterpriseServices.Atlas.Contracts;
using Moq;
using NUnit.Framework;
using Revampness.Services.device.TsmRptLibrary;
using System;
using System.Collections.Generic;

namespace TsmRptLibraryTests
{
    [TestFixture]
    public class AtlasConfigurationItemsWrapperTests
    {
        [Test]
        public void when_initialized_does_not_throw()
        {
            //Arrange
            var configHelper = new Mock<ConfigHelper>();
            var atlasClient = new Mock<IAtlasClient>();
            var patientGuid = Guid.NewGuid();

            //Act
            var sut = new AtlasConfigurationItemsWrapper(configHelper.Object, atlasClient.Object, patientGuid);

            //Assert
            Assert.Pass();
        }

        [Test]
        public void when_email_configured_setwindowscredentials_is_executed()
        {
            //Arrange
            var configHelper = new Mock<ConfigHelper>();
            var atlasClient = new Mock<IAtlasClient>();
            var patientGuid = Guid.NewGuid();
            var sut = new AtlasConfigurationItemsWrapper(configHelper.Object, atlasClient.Object, patientGuid);

            configHelper.SetupGet(x => x.IsEmailConfigured).Returns(true);
            configHelper.SetupGet(x => x.DomainUserName).Returns(It.IsAny<string>);
            configHelper.SetupGet(x => x.DomainPassword).Returns(It.IsAny<string>);
            configHelper.SetupGet(x => x.Domain).Returns(It.IsAny<string>);

            atlasClient.Setup(x => x.SetWindowsCredentials(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Verifiable();

            //Act
            sut.FetchAtlasSettings();
            //Assert
            atlasClient.Verify();
        }

        [Test]
        public void when_email_not_configured_GetCurrentValuesNonStatic_is_executed()
        {
            //Arrange
            var configHelper = new Mock<ConfigHelper>();
            var atlasClient = new Mock<IAtlasClient>();
            var patientGuid = Guid.NewGuid();
            var sut = new AtlasConfigurationItemsWrapper(configHelper.Object, atlasClient.Object, patientGuid);

            configHelper.SetupGet(x => x.IsEmailConfigured).Returns(false);
            atlasClient.Setup(x => x.GetCurrentValuesNonStatic(It.IsAny<HierarchyNodeReference>())).Verifiable();

            //Act
            sut.FetchAtlasSettings();
            //Assert
            atlasClient.Verify();
        }

        [Test]
        public void sets_EnableTimedEvents_to_true_when_atlas_setting_is_true()
        {
            //Arrange
            var configHelper = new Mock<ConfigHelper>();
            var atlasClient = new Mock<IAtlasClient>();
            var patientGuid = Guid.NewGuid();

            var application = ApplicationReference.ByName("Reporting");
            var area = AreaReference.ByName(application, "Default");
            var level = HierarchyLevelReference.ByName(area, "Patient");
            var node = HierarchyNodeReference.ByExternalKey(level, Guid.NewGuid().ToString());

            var targetSetting = new SettingValueBinding
            {
                HierarchyNode = node,
                Setting = new SettingReference { Name = "Timed" },
                Value = "true",
            };

            var settingsbinding = new List<SettingValueBinding>
                                {
                                    targetSetting
                                };

            var settings = new SettingValueBindingCollection(settingsbinding);
            var sut = new AtlasConfigurationItemsWrapper(configHelper.Object, atlasClient.Object, patientGuid, settings);

            configHelper.SetupGet(x => x.IsEmailConfigured).Returns(false);

            //Act
            sut.UpdatePropertiesBasedOnAtlasSettings();

            //Assert
            Assert.That(sut.EnableTimedEvents.Equals(true));
        }

        [Test]
        public void sets_TimedDailyFrequency_to_3_when_atlas_setting_is_3()
        {
            //Arrange
            var configHelper = new Mock<ConfigHelper>();
            var atlasClient = new Mock<IAtlasClient>();
            var patientGuid = Guid.NewGuid();

            var application = ApplicationReference.ByName("Reporting");
            var area = AreaReference.ByName(application, "Default");
            var level = HierarchyLevelReference.ByName(area, "Patient");
            var node = HierarchyNodeReference.ByExternalKey(level, Guid.NewGuid().ToString());

            var targetSetting = new SettingValueBinding
            {
                HierarchyNode = node,
                Setting = new SettingReference { Name = "TimedDailyFrequency" },
                Value = "3",
            };

            var settingsbinding = new List<SettingValueBinding>
                                {
                                    targetSetting
                                };

            var settings = new SettingValueBindingCollection(settingsbinding);
            var sut = new AtlasConfigurationItemsWrapper(configHelper.Object, atlasClient.Object, patientGuid, settings);

            configHelper.SetupGet(x => x.IsEmailConfigured).Returns(false);

            //Act
            sut.UpdatePropertiesBasedOnAtlasSettings();

            //Assert
            Assert.That(sut.TimedDailyFrequency.Equals(3));
        }

        [Test]
        public void sets_TimedDailyFrequency_to_1_when_atlas_setting_is_not_integer()
        {
            //Arrange
            var configHelper = new Mock<ConfigHelper>();
            var atlasClient = new Mock<IAtlasClient>();
            var patientGuid = Guid.NewGuid();

            var application = ApplicationReference.ByName("Reporting");
            var area = AreaReference.ByName(application, "Default");
            var level = HierarchyLevelReference.ByName(area, "Patient");
            var node = HierarchyNodeReference.ByExternalKey(level, Guid.NewGuid().ToString());

            var targetSetting = new SettingValueBinding
            {
                HierarchyNode = node,
                Setting = new SettingReference { Name = "TimedDailyFrequency" },
                Value = "thisisnotaninteger",
            };

            var settingsbinding = new List<SettingValueBinding>
                                {
                                    targetSetting
                                };

            var settings = new SettingValueBindingCollection(settingsbinding);
            var sut = new AtlasConfigurationItemsWrapper(configHelper.Object, atlasClient.Object, patientGuid, settings);

            configHelper.SetupGet(x => x.IsEmailConfigured).Returns(false);

            //Act
            sut.UpdatePropertiesBasedOnAtlasSettings();

            //Assert
            Assert.That(sut.TimedDailyFrequency.Equals(1));
        }

        [Test]
        public void sets_TimedStripsPerDay_to_12_when_atlas_setting_is_12()
        {
            //Arrange
            var configHelper = new Mock<ConfigHelper>();
            var atlasClient = new Mock<IAtlasClient>();
            var patientGuid = Guid.NewGuid();

            var application = ApplicationReference.ByName("Reporting");
            var area = AreaReference.ByName(application, "Default");
            var level = HierarchyLevelReference.ByName(area, "Patient");
            var node = HierarchyNodeReference.ByExternalKey(level, Guid.NewGuid().ToString());

            var targetSetting = new SettingValueBinding
            {
                HierarchyNode = node,
                Setting = new SettingReference { Name = "TimedStripsPerDay" },
                Value = "12",
            };

            var settingsbinding = new List<SettingValueBinding>
                                {
                                    targetSetting
                                };

            var settings = new SettingValueBindingCollection(settingsbinding);
            var sut = new AtlasConfigurationItemsWrapper(configHelper.Object, atlasClient.Object, patientGuid, settings);

            configHelper.SetupGet(x => x.IsEmailConfigured).Returns(false);

            //Act
            sut.UpdatePropertiesBasedOnAtlasSettings();

            //Assert
            Assert.That(sut.TimedStripsPerDay.Equals(12));
        }

        [Test]
        public void sets_TimedStripHour_to_6_when_atlas_setting_is_incorrect_formatted()
        {
            //Arrange
            var configHelper = new Mock<ConfigHelper>();
            var atlasClient = new Mock<IAtlasClient>();
            var patientGuid = Guid.NewGuid();

            var application = ApplicationReference.ByName("Reporting");
            var area = AreaReference.ByName(application, "Default");
            var level = HierarchyLevelReference.ByName(area, "Patient");
            var node = HierarchyNodeReference.ByExternalKey(level, Guid.NewGuid().ToString());

            var targetSetting = new SettingValueBinding
            {
                HierarchyNode = node,
                Setting = new SettingReference { Name = "TimedStripHour" },
                Value = "1998-01-01 23:59:59.997",
            };

            var settingsbinding = new List<SettingValueBinding>
                                {
                                    targetSetting
                                };

            var settings = new SettingValueBindingCollection(settingsbinding);
            var sut = new AtlasConfigurationItemsWrapper(configHelper.Object, atlasClient.Object, patientGuid, settings);

            configHelper.SetupGet(x => x.IsEmailConfigured).Returns(false);

            //Act
            sut.UpdatePropertiesBasedOnAtlasSettings();

            //Assert
            Assert.That(sut.TimedStripHour.Equals(6));
        }

        [Test]
        public void sets_TimedStripHour_to_hour_when_atlas_setting_is_correct_formatted()
        {
            //Arrange
            var configHelper = new Mock<ConfigHelper>();
            var atlasClient = new Mock<IAtlasClient>();
            var patientGuid = Guid.NewGuid();

            var application = ApplicationReference.ByName("Reporting");
            var area = AreaReference.ByName(application, "Default");
            var level = HierarchyLevelReference.ByName(area, "Patient");
            var node = HierarchyNodeReference.ByExternalKey(level, Guid.NewGuid().ToString());

            var targetSetting = new SettingValueBinding
            {
                HierarchyNode = node,
                Setting = new SettingReference { Name = "TimedStripHour" },
                Value = "23:59",
            };

            var settingsbinding = new List<SettingValueBinding>
                                {
                                    targetSetting
                                };

            var settings = new SettingValueBindingCollection(settingsbinding);
            var sut = new AtlasConfigurationItemsWrapper(configHelper.Object, atlasClient.Object, patientGuid, settings);

            configHelper.SetupGet(x => x.IsEmailConfigured).Returns(false);

            //Act
            sut.UpdatePropertiesBasedOnAtlasSettings();

            //Assert
            Assert.That(sut.TimedStripHour.Equals(23));
        }

        [Test]
        public void when_timedstripsperday_is_1_and_run_timedstripsperday_then_timedstripsperday_is_1()
        {
            //Arrange
            var configHelper = new Mock<ConfigHelper>();
            var atlasClient = new Mock<IAtlasClient>();
            var patientGuid = Guid.NewGuid();
            var sut = new AtlasConfigurationItemsWrapper(configHelper.Object, atlasClient.Object, patientGuid);
            sut.TimedStripsPerDay = 1;

            //Act
            sut.UpdateTimedStrips();

            //Assert
            Assert.That(() => sut.TimedStripsPerDay.Equals(1));
        }

        [Test]
        public void when_TimedStripsPerDay_is_11_then_CalculateTimedStripsHourIncrementor_is_4()
        {
            //Arrange
            var configHelper = new Mock<ConfigHelper>();
            var atlasClient = new Mock<IAtlasClient>();
            var patientGuid = Guid.NewGuid();
            var sut = new AtlasConfigurationItemsWrapper(configHelper.Object, atlasClient.Object, patientGuid);
            sut.TimedStripsPerDay = 11;

            //Act

            //Assert
            Assert.That(() => sut.CalculateTimedStripsHourIncrementor.Equals(4));
        }

        /*[Test]
        public void Does_not_throw_when_class_initialized_with_nonNull_parameters()
        {
            //Arrange
            var configHelperMock = Mock.Of<ConfigHelper>();
            var anyDatetime = DateTime.Now;
            //Act
            var cut = new QueueOperator(anyDatetime, configHelperMock);
            //Assert
            Assert.Pass();
        }

        [Test]
        public void Throws_MissingAppSettingException_when_configHelper_parameter_is_null()
        {
            //Arrange
            var anyDatetime = DateTime.Now;
            //Act
            //Assert
            Assert.Throws(Is.InstanceOf<MissingAppSettingException>(), () => new QueueOperator(anyDatetime, null));
        }

        [Test]
        public void PopMessageOffQueue_returns_false_when_messagequeueReceive_is_not_null()
        {
            //Arrange
            var mockMessageQueue = new Mock<IMessageQueue>();
            var mockConfigHelper = Mock.Of<ConfigHelper>();
            var anyDatetime = DateTime.Now;

            mockMessageQueue.Setup(x => x.Receive(TimeSpan.Zero)).Returns(() => new Message());
            //Act
            var cut = new QueueOperator(anyDatetime, mockConfigHelper);

            //Assert
            Assert.That(cut.PopMessageOffQueue(mockMessageQueue.Object).Equals(false));
        }

        [Test]
        public void PopMessageOffQueue_returns_true_when_messagequeueReceive_is_null()
        {
            //Arrange
            var mockMessageQueue = new Mock<IMessageQueue>();
            var mockConfigHelper = Mock.Of<ConfigHelper>();
            var anyDatetime = DateTime.Now;

            mockMessageQueue.Setup(x => x.Receive(TimeSpan.Zero)).Returns(() => null);
            //Act
            var cut = new QueueOperator(anyDatetime, mockConfigHelper);

            //Assert
            Assert.That(cut.PopMessageOffQueue(mockMessageQueue.Object).Equals(true));
        }

        [Test]
        public void GetNextMessage_returns_null_when_process_is_old()
        {
            //Arrange
            var mockMessageQueue = new Mock<IMessageQueue>();
            var mockConfigHelper = Mock.Of<ConfigHelper>();
            var anyDatetime = DateTime.Now;
            var startupAutomationMessage = new Mock<IStartupAutomationMessage>();
            var dataContract = new DataContractJsonSerializer(typeof(StartupAutomationWrapper));

            startupAutomationMessage.SetupGet(x => x.DeserializedMessage(dataContract).LastAttemptDate).Returns(() => DateTime.MinValue);
            mockMessageQueue.Setup(x => x.Peek(TimeSpan.Zero)).Returns(() => new Message());
            //Act
            var cut = new QueueOperator(anyDatetime, mockConfigHelper);
            var getNextMessage = cut.GetNextMessage(mockMessageQueue.Object, startupAutomationMessage.Object);

            //Assert
            Assert.That(getNextMessage != null);
        }*/
    }
}