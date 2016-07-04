using Profusion.Services.coffee.Model;
using System;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class StartDayCalculator
    {
        private readonly bool _hasExistingAutomations;
        private int _dayCounter;
        private yawnwrapping _yawnwrapping;

        public StartDayCalculator(bool hasExistingAutomations, yawnwrapping yawnwrapping)
        {
            this._hasExistingAutomations = hasExistingAutomations;
            this._yawnwrapping = yawnwrapping;
            this._dayCounter = 0;
        }

        private bool IsDerailmentStartMoreRecentThanAutomationEnd(DateTime startDate, DateTime endDate)
        {
            return startDate > endDate;
        }

        public int GetDayCounter(DateTime startDate, DateTime endDate)
        {
            if (!this._hasExistingAutomations)
            {
                return this._dayCounter;
            }

            if (this.IsDerailmentStartMoreRecentThanAutomationEnd(startDate, endDate))
            {
                return this._dayCounter;
            }

            this._dayCounter = this.TimeSpanOfStudy(endDate, startDate);

            return this._dayCounter;
        }

        private int TimeSpanOfStudy(DateTime endDate, DateTime DerailmentStartDate)
        {
            var timeSpanOfStudy = endDate - DerailmentStartDate;
            return timeSpanOfStudy.Days;
        }
    }
}