using NUnit.Framework;
using System.Configuration;

namespace TsmRptLibraryTests
{
    [TestFixture]
    public class ConfigValuesTests
    {
        [Test]
        public void Assert_that_config_value_isnotnull()
        {
            // the default app.config is used.
            var appconfig = @"..\..\..\SettingsQueueService\App.config";
            using (AppConfigHelper.Change(appconfig))
            {
                Assert.IsFalse(string.IsNullOrEmpty(ConfigurationManager.AppSettings["OtaQueue"]), "OtaQueue not found in config file.");
                Assert.IsFalse(string.IsNullOrEmpty(ConfigurationManager.AppSettings["log4net.Internal.Debug"]), "log4net.Internal.Debug not found in config file.");
                Assert.IsFalse(string.IsNullOrEmpty(ConfigurationManager.AppSettings["ReQueueService.Interval.ms"]), "ReQueueService.Interval.ms not found in config file.");
                Assert.IsFalse(string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServiceBus.Logging"]), "ServiceBus.Logging not found in config file.");
                Assert.IsFalse(string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServiceBus.ReceiveFromAddress"]), "ServiceBus.ReceiveFromAddress not found in config file.");
                Assert.IsFalse(string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServiceBus.SubscriptionServiceAddress"]), "ServiceBus.SubscriptionServiceAddress not found in config file.");
                Assert.IsFalse(string.IsNullOrEmpty(ConfigurationManager.AppSettings["GPStaging.ServiceBus.SubscriptionServiceAddress"]), "GPStaging.ServiceBus.SubscriptionServiceAddress not found in config file.");
                Assert.IsFalse(string.IsNullOrEmpty(ConfigurationManager.AppSettings["GPStaging.ServiceBus.ReceiveFromAddress"]), "GPStaging.ServiceBus.ReceiveFromAddress not found in config file.");
                Assert.IsFalse(string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServiceBus.Enabled"]), "ServiceBus.Enabled not found in config file.");
                Assert.IsFalse(string.IsNullOrEmpty(ConfigurationManager.AppSettings["OtaQueue"]), "OtaQueue not found in config file.");
                Assert.IsFalse(string.IsNullOrEmpty(ConfigurationManager.AppSettings["OtaArchiveRoot"]), "OtaArchiveRoot not found in config file.");
                Assert.IsFalse(string.IsNullOrEmpty(ConfigurationManager.AppSettings["MassTransitProbe"]), "MassTransitProbe not found in config file.");
                Assert.IsFalse(string.IsNullOrEmpty(ConfigurationManager.AppSettings["OtaArchiveRoot"]), "OtaArchiveRoot not found in config file.");
                Assert.IsFalse(string.IsNullOrEmpty(ConfigurationManager.AppSettings["deviceSettingsQueueRepositoryFolder"]), "deviceSettingsQueueRepositoryFolder not found in config file.");
                Assert.IsFalse(string.IsNullOrEmpty(ConfigurationManager.AppSettings["deviceSettingsCompleteRepositoryFolder"]), "deviceSettingsCompleteRepositoryFolder not found in config file.");
                Assert.IsFalse(string.IsNullOrEmpty(ConfigurationManager.AppSettings["ClientSettingsProvider.ServiceUri"]), "ClientSettingsProvider.ServiceUri not found in config file.");
            }
            // the default app.config is used.

            Assert.Pass();
        }
    }
}