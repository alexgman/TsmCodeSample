using NUnit.Framework;
using Profusion.Services.coffee.OsdRptLibrary;
using System;

namespace OsdRptLibraryTests
{
    [TestFixture]
    public class yawnwrappingCheckerTests
    {
        [Test]
        public void when_Connectionstring_is_nullorempty_throws_configurationerrorsexception()
        {
            Assert.That(() => new yawnwrappingChecker(""), Throws.InstanceOf<ArgumentException>());
        }

        [Test]
        public void when_ptguid_is_empty_throw_argumentoutofrange()
        {
            var sut = new yawnwrappingChecker("not null connectionstring");
            Assert.That(() => sut.GetLastyawnwrappingId(Guid.Empty, "111111"), Throws.InstanceOf<ArgumentException>());
        }

        [Test]
        public void when_serialnumber_is_blank_then_throw_argumentoutofrange()
        {
            var sut = new yawnwrappingChecker("not null connectionstring");
            Assert.That(() => sut.GetLastyawnwrappingId(Guid.NewGuid(), ""), Throws.InstanceOf<ArgumentException>());
        }
    }
}