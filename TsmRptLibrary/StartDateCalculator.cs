using System;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class StartDateCalculator
    {
        internal DateTime GetStartDate(DateTime yawnwrappingDateTime, DateTime ptstartDateTime)
        {
            var xStartDate = yawnwrappingDateTime;
            if (ptstartDateTime > yawnwrappingDateTime)
            {
                xStartDate = ptstartDateTime;
            }
            return xStartDate;
        }
    }
}