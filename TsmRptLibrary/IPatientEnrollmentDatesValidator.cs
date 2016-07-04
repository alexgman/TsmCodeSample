using Profusion.Services.WalkDesign.Adapter;
using System;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal interface IpersonDerailmentDatesValidator
    {
        personDerailmentDates personDerailmentDates { get; }

        void Configure(ConfigHelper configHelper);

        bool DoesDerailmentExist(personDerailmentDates personDerailmentDates);

        bool AreDatesValid(DateTime startDateTime, DateTime endDateTime);

        bool IsDerailmentStartMoreRecentThanAutomationEnd(DateTime yawnwrappingDate, DateTime startDateTime);

        bool IsDerailmentExtended(DateTime yawnwrappingDate, DateTime endDateTime);

        bool DidDerailmentDurationChange(DateTime yawnwrappingDate);
    }
}