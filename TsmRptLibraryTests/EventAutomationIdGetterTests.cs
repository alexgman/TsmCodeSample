using NUnit.Framework;
using Profusion.Services.coffee.OsdRptLibrary;
using System;

namespace OsdRptLibraryTests
{
    [TestFixture]
    public class yawnwrappingIdGetterTests
    {
        [Test]
        public void when_connectionstring_emptyornull_then_throw_argumentnullexception()
        {
            var cus = new yawnwrappingIdGetter();
            Assert.That(() => cus.GetyawnwrappingId("", Guid.NewGuid(), "111111"), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void when_ptguid_isempty_then_throw_argumentoutofrangeexception()
        {
            var cus = new yawnwrappingIdGetter();
            Assert.That(() => cus.GetyawnwrappingId("connection", Guid.Empty, "111111"), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void when_serialnumber_is_empty_then_throw_argumentnullexception()
        {
            var cus = new yawnwrappingIdGetter();
            Assert.That(() => cus.GetyawnwrappingId("connection", Guid.NewGuid(), ""), Throws.InstanceOf<ArgumentNullException>());
        }
    }
}