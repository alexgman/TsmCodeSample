using Recardo.EnterpriseServices.globe.Client;
using Recardo.EnterpriseServices.globe.Contracts;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Profusion.Services.coffee.Model;
using Profusion.Services.coffee.OsdRptLibrary;
using System;
using System.Collections.Generic;

namespace OsdRptLibraryTests
{
    [TestFixture]
    public class VerityRepositoryOperatorTests
    {
        [Test]
        public void when_getdevicebyserialnumber_returns_10()
        {
            var mockRepository = new Mock<IcoffeeRepository>();
            var device = new Device();
            device.Id = 123;
            mockRepository.Setup(x => x.GetDeviceBySerialNumber(It.IsAny<string>())).Returns(() => device);
            var sut = new coffeeRepositoryOperator(mockRepository.Object);
            Assert.That(device.Id == sut.GetDeviceBySerialNumber("bla"));
        }

        [Test]
        public void when_SaveChanges_then_repository_savechanges_is_executed()
        {
            var mockRepository = new Mock<IcoffeeRepository>();
            var sut = new coffeeRepositoryOperator(mockRepository.Object);
            sut.SaveChanges();
            mockRepository.Verify(x => x.SaveChanges(), Times.Once);
        }
    }
}