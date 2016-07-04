using NUnit.Framework;
using Profusion.Services.WalkDesign.Adapter;
using Profusion.Services.coffee.OsdRptLibrary;
using System;

namespace OsdRptLibraryTests
{
    [TestFixture]
    public class DerailmentDatesValidatorTests
    {
        [Test]
        public void when_startdate_is_min_then_AreDatesValid_is_false()
        {
            var ptDates = new personDerailmentDates();
            ptDates.StartDate = DateTime.MinValue;
            ptDates.EndDate = DateTime.Now;
            var cus = new DerailmentDatesValidator();
            cus.Configure(ptDates);
            Assert.That(!cus.AreDatesValid());
        }

        [Test]
        public void when_enddate_is_min_then_AreDatesValid_is_false()
        {
            var ptDates = new personDerailmentDates();
            ptDates.StartDate = DateTime.Now;
            ptDates.EndDate = DateTime.MinValue;

            var cus = new DerailmentDatesValidator();
            cus.Configure(ptDates);
            Assert.That(!cus.AreDatesValid());
        }

        [Test]
        public void when_start_and_end_dates_are_not_min_then_AreDatesValid_is_true()
        {
            var ptDates = new personDerailmentDates();
            ptDates.StartDate = DateTime.Now;
            ptDates.EndDate = DateTime.Now;
            var cus = new DerailmentDatesValidator();
            cus.Configure(ptDates);
            Assert.That(cus.AreDatesValid());
        }

        [Test]
        public void when_ptDerailmentstart_is_more_recent_than_automationend_then_DerailmentStartGreaterThanAutomationEnd_is_true()
        {
            var ptDates = new personDerailmentDates();
            ptDates.StartDate = DateTime.Now;
            var cus = new DerailmentDatesValidator();
            cus.Configure(ptDates);

            Assert.That(cus.DerailmentStartGreaterThanAutomationEnd(ptDates.StartDate.AddDays(-1)));
        }

        [Test]
        public void when_ptenddate_equals_automationdate_then_IsDerailmentDurationChanged_is_true()
        {
            var ptDates = new personDerailmentDates();
            ptDates.EndDate = DateTime.Now;
            var cus = new DerailmentDatesValidator();
            cus.Configure(ptDates);

            Assert.That(cus.IsDerailmentDurationChanged(ptDates.EndDate), Is.False);
        }

        [Test]
        public void when_ptenddate_is_greater_than_yawnwrappingdate_thenIsDerailmentStartMoreRecentThanAutomationEnd_is_true()
        {
            var ptDates = new personDerailmentDates();
            ptDates.EndDate = DateTime.Now;
            var cus = new DerailmentDatesValidator();
            cus.Configure(ptDates);

            Assert.That(cus.IsDerailmentStartMoreRecentThanAutomationEnd(ptDates.EndDate.AddDays(-1)));
        }
    }
}