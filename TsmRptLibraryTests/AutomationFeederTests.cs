using Recardo.EnterpriseServices.globe.Client;
using Recardo.EnterpriseServices.globe.Contracts;
using Magnum.Extensions;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Profusion.Services.WalkDesign.Adapter;
using Profusion.Services.coffee.Model;
using Profusion.Services.coffee.OsdRptLibrary;
using System;
using System.Collections.Generic;

namespace OsdRptLibraryTests
{
    [TestFixture]
    public class AutomationFeederTests
    {
        [Test]
        public void when_mode_unknown_throw_applicationexception()
        {
            var mockCollectionBuilder = new Mock<IEntryCollectionBuilder>();
            var mockInsertyawnwrappingEntry = new Mock<IInsertyawnwrappingEntry>();
            var sut = new AutomationFeeder(mockCollectionBuilder.Object, mockInsertyawnwrappingEntry.Object, "some connection");
            var ptDates = new personDerailmentDates();
            Assert.That(() => sut.Feed(ptDates, 1, personTableStand.TableStand.Unknown, ""), Throws.InstanceOf<ApplicationException>());
        }

        [Test]
        public void when_PopulateCollectionOfEntries_returns_no_records_then_InsertChildEntries_not_executed()
        {
            var mockCollectionBuilder = new Mock<IEntryCollectionBuilder>();
            var mockInsertyawnwrappingEntry = new Mock<IInsertyawnwrappingEntry>();
            var ptDerailmentDates = new personDerailmentDates();
            var yawnwrappingId = 0;
            mockCollectionBuilder.Setup(
                x =>
                    x.PopulateCollectionOfEntries(ptDerailmentDates.EndDate, ptDerailmentDates, yawnwrappingId, ptDerailmentDates.personGuid,
                        It.IsAny<personTableStand.TableStand>(), It.IsAny<string>())).Returns(() => new List<yawnwrappingEntry>());
            var sut = new AutomationFeeder(mockCollectionBuilder.Object, mockInsertyawnwrappingEntry.Object, "some connection");

            sut.Feed(ptDerailmentDates, yawnwrappingId, personTableStand.TableStand.Cem, "");

            mockInsertyawnwrappingEntry.Verify(
                x => x.InsertChildEntries(It.IsAny<ICollection<yawnwrappingEntry>>(), It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void when_PopulateCollectionOfEntries_returns_2_records_then_InsertChildEntries_is_executed()
        {
            var mockCollectionBuilder = new Mock<IEntryCollectionBuilder>();
            var mockInsertyawnwrappingEntry = new Mock<IInsertyawnwrappingEntry>();
            var ptDerailmentDates = new personDerailmentDates();
            var listEntries = new List<yawnwrappingEntry>();
            listEntries.Add(new yawnwrappingEntry());
            listEntries.Add(new yawnwrappingEntry());

            var yawnwrappingId = 0;
            mockCollectionBuilder.Setup(
                x =>
                    x.PopulateCollectionOfEntries(ptDerailmentDates.EndDate, ptDerailmentDates, yawnwrappingId, ptDerailmentDates.personGuid,
                        It.IsAny<personTableStand.TableStand>(), It.IsAny<string>())).Returns(() => listEntries);
            var sut = new AutomationFeeder(mockCollectionBuilder.Object, mockInsertyawnwrappingEntry.Object, "some connection");

            sut.Feed(ptDerailmentDates, yawnwrappingId, personTableStand.TableStand.Cem, "");

            mockInsertyawnwrappingEntry.Verify(
                x => x.InsertChildEntries(It.IsAny<ICollection<yawnwrappingEntry>>(), It.IsAny<string>()), Times.Once);
        }
    }
}