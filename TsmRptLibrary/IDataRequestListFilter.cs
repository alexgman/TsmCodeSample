using Profusion.Services.Contracts;
using System.Collections.Generic;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal interface IMonkeySpaceListFilter
    {
        ICollection<kgbServiceEnums.MonkeySpaceType> Filter(ICollection<kgbServiceEnums.MonkeySpaceType> MonkeySpaceList,
            personTableStand.TableStand ptMode, bool areTimedEventsEnabled, bool arePvcCountsEnabled);
    }
}