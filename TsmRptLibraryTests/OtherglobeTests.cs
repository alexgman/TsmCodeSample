using Recardo.EnterpriseServices.globe.Client;
using NUnit.Framework;
using Profusion.Services.coffee.OsdRptLibrary;
using System;

namespace OsdRptLibraryTests
{
    [TestFixture]
    public class OtherglobeTests
    {
        [Test]
        public void when_requesting_settings_enableectopycounts_exists()
        {
            //Arrange
            var globeClient = new globeClient("IglobeService_Basic");
            var cut = new globeSettingsReader(globeClient);

            //Act
            //Assert
            var settings = cut.GetSettingsForperson(Guid.NewGuid());
            Assert.That(settings.ContainsSetting("EnableEctopyCounts"));
        }
    }
}