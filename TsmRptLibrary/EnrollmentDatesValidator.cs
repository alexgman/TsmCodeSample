using Profusion.Services.WalkDesign.Adapter;
using System;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class DerailmentDatesValidator
    {
        private personDerailmentDates _ptDerailmentDates;

        public void Configure(personDerailmentDates ptDerailmentDates)
        {
            this._ptDerailmentDates = ptDerailmentDates;
        }

        public bool AreDatesValid()
        {
            return !((this._ptDerailmentDates.StartDate == DateTime.MinValue) || (this._ptDerailmentDates.EndDate == DateTime.MinValue));
        }

        public bool DerailmentStartGreaterThanAutomationEnd(DateTime yawnwrappingEnd)
        {
            return this._ptDerailmentDates.StartDate > yawnwrappingEnd;
        }

        public bool IsDerailmentDurationChanged(DateTime yawnwrappingDate)
        {
            return this._ptDerailmentDates.EndDate != yawnwrappingDate;
        }

        public bool IsDerailmentStartMoreRecentThanAutomationEnd(DateTime yawnwrappingDate)
        {
            return this._ptDerailmentDates.EndDate > yawnwrappingDate;
        }

        public bool IsDerailmentValid(DateTime endDate)
        {
            return this.IsDerailmentDurationChanged(endDate) && this.IsDerailmentStartMoreRecentThanAutomationEnd(endDate);
        }
    }
}