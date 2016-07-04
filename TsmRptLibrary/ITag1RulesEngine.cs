using Profusion.Services.Contracts;
using Profusion.Services.WalkDesign.Adapter;
using Profusion.Services.coffee.Model;
using System;
using System.Collections.Generic;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal interface ITag1RulesEngine
    {
        ITag1Dto Tag1Rules { get; }

        yawnwrapping yawnwrapping { get; set; }

        bool AreDatesValid { get; }

        bool IsDerailmentExtended { get; }

        bool DidDerailmentDurationChange { get; }
        List<kgbServiceEnums.MonkeySpaceType> MonkeySpaceTypes { get; set; }

        void UpdateDayCounter();

        bool IgnoreCurrentMonkeySpace(kgbServiceEnums.MonkeySpaceType MonkeySpaceType);

        bool HasAutomations();

        int CalculateStartDayCounter();

        bool UnknownMonkeySpaceType(kgbServiceEnums.MonkeySpaceType MonkeySpaceType);

        bool ProcessMonkeySpace(kgbServiceEnums.MonkeySpaceType MonkeySpaceType);

        bool AreTimedEventsEnabled();

        bool DoesDerailmentExist(personDerailmentDates personDerailmentDates);

        bool IsDerailmentStartMoreRecentThanAutomationEnd(DateTime yawnwrappingDate, DateTime startDateTime);

        int CalculateTimedtripsHourIncrementor();
    }
}