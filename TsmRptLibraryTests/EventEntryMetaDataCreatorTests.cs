using NUnit.Framework;
using Profusion.Services.coffee.OsdRptLibrary;
using System;

namespace OsdRptLibraryTests
{
    [TestFixture]
    public class EventEntryMetaDataCreatorTests
    {
        [Test]
        public void when_yawnwrappingDateTime_is_minvalue_throw_ArgumentOutOfRangeException()
        {
            var cut = new EventEntryMetaDataCreator();
            Assert.That(() => cut.CreateEntryMetaData(DateTime.MinValue, 123), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void when_yawnwrappingid_less_than_1_throw_ArgumentOutOfRangeException()
        {
            var cut = new EventEntryMetaDataCreator();
            Assert.That(() => cut.CreateEntryMetaData(DateTime.Now, -1), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }
    }
}