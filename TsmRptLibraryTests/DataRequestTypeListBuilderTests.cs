using NUnit.Framework;
using Profusion.Services.Contracts;
using Profusion.Services.coffee.OsdRptLibrary;
using System.Collections.Generic;

namespace OsdRptLibraryTests
{
    [TestFixture]
    public class MonkeySpaceTypeListBuilderTests
    {
        [Test]
        public void when_mode_is_cem_then_return_timed()
        {
            var cus = new MonkeySpaceTypeListBuilder();
            var listOfEntries = cus.GetMonkeySpaceTypeList(personTableStand.TableStand.Cem);
            var expectedResult = new List<kgbServiceEnums.MonkeySpaceType>();
            expectedResult.Add(kgbServiceEnums.MonkeySpaceType.Timed);

            Assert.That(listOfEntries.Count == 1);
            Assert.That(listOfEntries.Contains(kgbServiceEnums.MonkeySpaceType.Timed));
        }

        [Test]
        public void when_mode_is_mct_then_return_min_max_timed_telemed()
        {
            var cus = new MonkeySpaceTypeListBuilder();
            var listOfEntries = cus.GetMonkeySpaceTypeList(personTableStand.TableStand.Mct);
            var expectedResult = new List<kgbServiceEnums.MonkeySpaceType>();
            expectedResult.Add(kgbServiceEnums.MonkeySpaceType.Timed);
            expectedResult.Add(kgbServiceEnums.MonkeySpaceType.MinimumHr);
            expectedResult.Add(kgbServiceEnums.MonkeySpaceType.MaximumHr);
            expectedResult.Add(kgbServiceEnums.MonkeySpaceType.Telemed);

            Assert.That(listOfEntries.Count == 4);
            Assert.That(listOfEntries.Contains(kgbServiceEnums.MonkeySpaceType.Timed));
            Assert.That(listOfEntries.Contains(kgbServiceEnums.MonkeySpaceType.MinimumHr));
            Assert.That(listOfEntries.Contains(kgbServiceEnums.MonkeySpaceType.MaximumHr));
            Assert.That(listOfEntries.Contains(kgbServiceEnums.MonkeySpaceType.Telemed));
        }
    }
}