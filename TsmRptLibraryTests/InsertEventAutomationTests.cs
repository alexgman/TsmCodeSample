using NUnit.Framework;
using Profusion.Services.coffee.OsdRptLibrary;

namespace OsdRptLibraryTests
{
    [TestFixture]
    public class InsertyawnwrappingTests
    {
        [Test]
        public void when_connectionstring_is_empty_then_throw_argumentnullexception
            ()
        {
            Assert.That(() => new Insertyawnwrapping(""), Throws.ArgumentNullException);
        }
    }
}