using Moq;
using NUnit.Framework;
using Profusion.Services.coffee.OsdRptLibrary;
using Profusion.Services.coffee.OsdRptLibrary.DIWebService;
using Profusion.Services.coffee.OsdRptLibrary.EppWebService;
using System;

namespace OsdRptLibraryTests
{
    [TestFixture]
    public class DeviceStatusReaderTests
    {
        [Test]
        public void GetpersonDeviceStatus_returns_instance_of_applicationexception_when_personguid_doesnotexist()
        {
            var mockEppService = new Mock<PhysicianPortalServiceContract>();
            var mockDi = new Mock<ITaskCreatorAndWorkerService>();

            var cus = new DeviceStatusReader(mockEppService.Object, mockDi.Object);

            Assert.That(() => cus.GetpersonDeviceStatus(Guid.NewGuid()), Throws.InstanceOf<ApplicationException>());
        }
    }
}