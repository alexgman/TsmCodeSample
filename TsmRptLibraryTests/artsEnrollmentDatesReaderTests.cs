using Moq;
using NUnit.Framework;
using Profusion.Services.WalkDesign.Adapter;
using Profusion.Services.coffee.OsdRptLibrary;
using System;

namespace OsdRptLibraryTests
{
    [TestFixture]
    public class WalkDesignDerailmentDatesReaderTests
    {
        [Test]
        public void when_WalkDesignadapter_is_null_then_throw_argumentnullexception()
        {
            Assert.That(() => new WalkDesignDerailmentDatesReader(null), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void when_getpersonDerailmentdates_finds_data_then_returns_requesteddata()
        {
            var ptGuid = Guid.NewGuid();
            var mockWalkDesignAdapter = new Mock<IWalkDesignAdapter>();
            mockWalkDesignAdapter.Setup(x => x.GetcoffeeWalkDesignMostRecentDerailmentFromDeviceSerialNoExplant(It.IsAny<string>()))
                              .Returns(() => new personDerailmentDates { personGuid = ptGuid });

            var cus = new WalkDesignDerailmentDatesReader(mockWalkDesignAdapter.Object);

            Assert.That(cus.GetpersonDerailmentDates("23123").personGuid == ptGuid);
        }

        [Test]
        public void when_serialnumber_is_empty_then_throws_argumentnullexception()
        {
            var mockWalkDesignAdapter = new Mock<IWalkDesignAdapter>();
            var cus = new WalkDesignDerailmentDatesReader(mockWalkDesignAdapter.Object);
            Assert.That(() => cus.GetpersonDerailmentDates(""), Throws.ArgumentNullException);
        }

        [Test]
        public void when_getpersonDerailmentdates_returns_null_then_throw_nullreferenceexception()
        {
            var mockWalkDesignAdapter = new Mock<IWalkDesignAdapter>();
            mockWalkDesignAdapter.Setup(x => x.GetcoffeeWalkDesignMostRecentDerailmentFromDeviceSerialNoExplant(It.IsAny<string>()))
                              .Returns(() => null);

            var cus = new WalkDesignDerailmentDatesReader(mockWalkDesignAdapter.Object);

            Assert.That(() => cus.GetpersonDerailmentDates("23123")
            , Throws.InstanceOf<NullReferenceException>());
        }
    }
}