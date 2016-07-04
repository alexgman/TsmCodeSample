using NUnit.Framework;
using Revampness.Services.device.TsmRptLibrary;

namespace TsmRptLibraryTests
{
    [TestFixture]
    public class NonCemEventEntryCreatorTests
    {
        [Test]
        public void when_datarequesttype_via_then_count_return_0()
        {
            var mockTagRules1Engine = new Mock<ITag1RulesEngine>();
            var mockEventAutomationEntryCreator = new Mock<IEventAutomationEntryCreator>();

            mockTagRules1Engine.SetupGet(x => x.Tag1Rules.DataRequestType).Returns(EcgServiceEnums.DataRequestType.Via);

            var cut = new NonCemEventEntryCreator();

            var result = cut.Create(mockTagRules1Engine.Object, mockEventAutomationEntryCreator.Object);

            Assert.That(result.Count == 0);
        }

        [Test]
        public void when_datarequesttype_telemed_and_AtlasEnableEctopyCounts_is_false_then_count_return_0()
        {
            var mockTagRules1Engine = new Mock<ITag1RulesEngine>();
            var mockEventAutomationEntryCreator = new Mock<IEventAutomationEntryCreator>();

            mockTagRules1Engine.SetupGet(x => x.Tag1Rules.DataRequestType).Returns(EcgServiceEnums.DataRequestType.Telemed);
            mockTagRules1Engine.SetupGet(x => x.Tag1Rules.AtlasEnabledEctopyCounts).Returns(false);

            var cut = new NonCemEventEntryCreator();

            var result = cut.Create(mockTagRules1Engine.Object, mockEventAutomationEntryCreator.Object);

            Assert.That(result.Count == 0);
        }
    }
}