using Moq;
using NUnit.Framework;
using Profusion.Services.coffee.Model;
using Profusion.Services.coffee.OsdRptLibrary;
using System;

namespace OsdRptLibraryTests
{
    [TestFixture]
    public class DeviceUpdaterTests
    {
        [Test]
        public void when_yawnwrapping_isnull_then_throw_argumentnullexception()
        {
            var mockcoffeeRepository = new Mock<IcoffeeRepository>();
            var cus = new DeviceUpdater();

            Assert.Throws<ArgumentNullException>(() =>
            cus.UpdateDeviceId(null, "blah", mockcoffeeRepository.Object));
        }

        [Test]
        public void when_serialnumber_isempty_then_throw_argumentnullexception()
        {
            var mockcoffeeRepository = new Mock<IcoffeeRepository>();
            var cus = new DeviceUpdater();

            Assert.Throws<ArgumentNullException>(() => cus.UpdateDeviceId(new yawnwrapping(), "", mockcoffeeRepository.Object));
        }

        [Test]
        public void when_eventauftomation_isnull_then_throw_argumentnullexception()
        {
            var cus = new DeviceUpdater();

            Assert.Throws<ArgumentNullException>(() =>
            cus.UpdateDeviceId(new yawnwrapping(), "blah", null));
        }
    }
}