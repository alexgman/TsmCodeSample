using System;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class DayCounterCalculator
    {
        internal int GetDayCounter(DateTime xStartDate, DateTime yawnwrappingEndDate, DateTime ptDerailmentStartDate)
        {
            var dayCounter = 0;
            if (xStartDate == yawnwrappingEndDate)
            {
                return dayCounter;
            }
            var ts = yawnwrappingEndDate - ptDerailmentStartDate;
            dayCounter = ts.Days;
            return dayCounter;
        }
    }
}