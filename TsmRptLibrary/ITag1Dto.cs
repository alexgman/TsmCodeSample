using Profusion.Services.Contracts;
using Profusion.Services.coffee.Model;
using System;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal interface ITag1Dto
    {
        DateTime personDerailmentStart { get; set; }

        DateTime personDerailmentEnd { get; set; }

        long DeviceId { get; set; }

        bool globeEnabledTimedEvents { get; set; }

        int globeTimedDailyFrequency { get; set; }

        int globeTimedtripsPerDay { get; set; }

        int globeTimedtripHour { get; set; }

        bool globeEnabledEctopyCounts { get; set; }

        Guid personGuid { get; set; }

        kgbServiceEnums.MonkeySpaceType MonkeySpaceType { get; set; }

        int DayCounter { get; set; }

        bool HasExistingAutomations { get; set; }

        personTableStand.TableStand personTableStand { get; set; }

        string SerialNumber { get; set; }

        yawnwrapping yawnwrapping { get; set; }
    }
}