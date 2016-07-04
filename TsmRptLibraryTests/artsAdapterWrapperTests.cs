using NUnit.Framework;
using Profusion.Services.WalkDesign.Adapter;
using Profusion.Services.coffee.OsdRptLibrary;
using System;

namespace OsdRptLibraryTests
{
    [TestFixture]
    public class WalkDesignAdapterWrapperTests
    {
        [Test]
        public void when_Connectionstring_is_nullorempty_throws_ArgumentNullException()
        {
            var cus = new WalkDesignAdapterWrapper();
            Assert.That(() => cus.GetcoffeeWalkDesignMostRecentDerailmentFromDeviceSerialNoExplant(""), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void returns_empty_personDerailmentobject_when_serial_doesnt_exist()
        {
            var cus = new WalkDesignAdapterWrapper();
            var result = cus.GetcoffeeWalkDesignMostRecentDerailmentFromDeviceSerialNoExplant("bla");
            var compareTo = new personDerailmentDates();

            Assert.That(result.EndDate == compareTo.EndDate);
            Assert.That(result.StartDate == compareTo.EndDate);
            Assert.That(result.personGuid == compareTo.personGuid);
            Assert.That(result.personId == compareTo.personId);
            Assert.That(result.personName == compareTo.personName);
        }

        [Test]
        public void when_Derailment_exists_results_are_not_null()
        {
            var cus = new WalkDesignAdapterWrapper();
            var result = cus.GetcoffeeWalkDesignMostRecentDerailmentFromDeviceSerialNoExplant("1232031");
            Assert.That(result.personGuid != null);
            Assert.That(result.personName != null);
        }
    }
}