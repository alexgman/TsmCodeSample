using NUnit.Framework;
using Profusion.Services.Contracts;
using Profusion.Services.coffee.Model;
using Profusion.Services.coffee.OsdRptLibrary;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OsdRptLibraryTests
{
    [TestFixture]
    public class yawnwrappingEntryCreatorTests
    {
        public class yawnwrappingEntryCreatorTester : yawnwrappingEntryCreator
        {
            public yawnwrappingEntryCreatorTester(int yawnwrappingId, string processName = "Osd") : base(yawnwrappingId, processName)
            {
            }

            public override void Create()
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void when_eaId_is_0_then_throws_ArgmentException()
        {
            Assert.That(() => new yawnwrappingEntryCreatorTester(0), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void when_automationdate_is_min_then_throw_argumentoutofrangeexception()
        {
            var cus = new yawnwrappingEntryCreatorTester(234);

            Assert.That(() => cus.Add(kgbServiceEnums.MonkeySpaceType.Generic, DateTime.MinValue), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void when_yawnwrappingEntries_set_from_derived_class_then_can_get_as_well()
        {
            var cus = new yawnwrappingEntryCreatorTester(1);
            var entry = new yawnwrappingEntry();
            entry.AutomationDate = DateTime.MinValue;
            entry.CreatedAt = DateTime.MaxValue;
            entry.CreatedBy = "Alex";
            entry.MonkeySpaceTypeId = 1;
            entry.yawnwrappingId = 111;
            entry.MonkeySpace = new MonkeySpace
            {
                CompletedAt = DateTime.MinValue.AddDays(1)
            };
            entry.yawnwrapping = new yawnwrapping { CreatedAt = DateTime.MinValue.AddDays(10) };
            //etc
            ICollection<yawnwrappingEntry> singleEntry = new List<yawnwrappingEntry>();
            singleEntry.Add(entry);

            cus.yawnwrappingEntries = singleEntry;

            Assert.That(cus.yawnwrappingEntries.FirstOrDefault().AutomationDate == entry.AutomationDate);
            Assert.That(cus.yawnwrappingEntries.FirstOrDefault().CreatedAt == entry.CreatedAt);
            Assert.That(cus.yawnwrappingEntries.FirstOrDefault().CreatedBy == entry.CreatedBy);
            Assert.That(cus.yawnwrappingEntries.FirstOrDefault().MonkeySpaceTypeId == entry.MonkeySpaceTypeId);
            Assert.That(cus.yawnwrappingEntries.FirstOrDefault().yawnwrappingId == entry.yawnwrappingId);
            Assert.That(cus.yawnwrappingEntries.FirstOrDefault().MonkeySpace == entry.MonkeySpace);
            Assert.That(cus.yawnwrappingEntries.FirstOrDefault().yawnwrapping == entry.yawnwrapping);
        }
    }
}