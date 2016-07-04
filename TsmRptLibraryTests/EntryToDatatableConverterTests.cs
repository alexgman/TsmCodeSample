using NUnit.Framework;
using Profusion.Services.coffee.Model;
using Profusion.Services.coffee.OsdRptLibrary;
using System.Collections.Generic;

namespace OsdRptLibraryTests
{
    [TestFixture]
    public class EntryToDatatableConverterTests
    {
        [Test]
        public void when_automationEntry_list_is_empty_EntriesToDatatable_returns_empty_dt()
        {
            var cus = new EntryToDatatableConverter();
            var entryCollection = new List<yawnwrappingEntry>();
            Assert.That(cus.EntriesToDatatable(entryCollection).Rows.Count == 0);
        }

        [Test]
        public void when_automationEntries_is_null_then_throws_argumentnullexception()
        {
            var cus = new EntryToDatatableConverter();
            Assert.That(() => cus.EntriesToDatatable(null), Throws.ArgumentNullException);
        }
    }
}