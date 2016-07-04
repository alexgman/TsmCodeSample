using Moq;
using NUnit.Framework;
using Revampness.Services.device.Model;
using Revampness.Services.device.TsmRptLibrary;
using System.Collections.Generic;

namespace TsmRptLibraryTests
{
    [TestFixture]
    public class DelegateCreationBasedOnModeTests
    {
        [Test]
        public void when_cem_mode_then_executes_create_in_cemevententrycreator()
        {
            var mockNonCemEventEntryCreator = new Mock<INonCemEventEntryCreator>();
            var mockCemEventEntryCreator = new Mock<ICemEventEntryCreator>();
            var mockTag1Rules = new Mock<ITag1RulesEngine>();

            mockTag1Rules.SetupGet(x => x.Tag1Rules.PatientServiceMode).Returns(PatientServiceMode.ServiceMode.Cem);

            mockCemEventEntryCreator.Setup(x => x.Create(mockTag1Rules.Object, It.IsAny<TimedEntryCreator>())).Verifiable();

            var cut = new DelegateCreationBasedOnMode(mockNonCemEventEntryCreator.Object, mockCemEventEntryCreator.Object);
            cut.Start(mockTag1Rules.Object);

            mockCemEventEntryCreator.Verify();
        }

        [Test]
        public void when_cem_mode_then_returns_instance_of_icollection_eventautomationentry()
        {
            var mockNonCemEventEntryCreator = new Mock<INonCemEventEntryCreator>();
            var mockCemEventEntryCreator = new Mock<ICemEventEntryCreator>();
            var mockTag1Rules = new Mock<ITag1RulesEngine>();

            mockTag1Rules.SetupGet(x => x.Tag1Rules.PatientServiceMode).Returns(PatientServiceMode.ServiceMode.Cem);

            mockCemEventEntryCreator.Setup(x => x.Create(mockTag1Rules.Object, It.IsAny<TimedEntryCreator>())).Returns(() => new List<EventAutomationEntry>());

            var cut = new DelegateCreationBasedOnMode(mockNonCemEventEntryCreator.Object, mockCemEventEntryCreator.Object);
            var resultFromStart = cut.Start(mockTag1Rules.Object);

            Assert.That(resultFromStart, Is.InstanceOf<ICollection<EventAutomationEntry>>());
        }

        [Test]
        public void when_not_cem_mode_then_executes_create_in_Noncemevententrycreator()
        {
            var mockNonCemEventEntryCreator = new Mock<INonCemEventEntryCreator>();
            var mockCemEventEntryCreator = new Mock<ICemEventEntryCreator>();
            var mockTag1Rules = new Mock<ITag1RulesEngine>();

            mockTag1Rules.SetupGet(x => x.Tag1Rules.PatientServiceMode).Returns(PatientServiceMode.ServiceMode.Common);

            mockNonCemEventEntryCreator.Setup(x => x.Create(mockTag1Rules.Object, It.IsAny<EventAutomationEntryCreator>())).Verifiable();

            var cut = new DelegateCreationBasedOnMode(mockNonCemEventEntryCreator.Object, mockCemEventEntryCreator.Object);
            cut.Start(mockTag1Rules.Object);

            mockCemEventEntryCreator.Verify();
        }
    }
}