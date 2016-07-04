using System;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal interface IpersonDerailmentDates
    {
        DateTime EndDate { get; set; }
        Guid personGuid { get; set; }
        string personId { get; set; }
        string personName { get; set; }
        DateTime StartDate { get; set; }
    }
}