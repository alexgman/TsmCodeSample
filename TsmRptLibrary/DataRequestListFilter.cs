using Profusion.Services.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class MonkeySpaceListFilter : IMonkeySpaceListFilter
    {
        public ICollection<kgbServiceEnums.MonkeySpaceType> Filter(ICollection<kgbServiceEnums.MonkeySpaceType> MonkeySpaceList, personTableStand.TableStand ptMode,
            bool areTimedEventsEnabled, bool arePvcCountsEnabled)
        {
            return ptMode == personTableStand.TableStand.Cem
                ? this.FilterCem(MonkeySpaceList, areTimedEventsEnabled)
                : this.FilterNonCem(MonkeySpaceList, areTimedEventsEnabled, arePvcCountsEnabled);
        }

        private ICollection<kgbServiceEnums.MonkeySpaceType> FilterCem(ICollection<kgbServiceEnums.MonkeySpaceType> thisMonkeySpaceList, bool areTimedEventsEnabled)
        {
            var filteredTimedRequests = this.FilterTimedRequests(thisMonkeySpaceList, areTimedEventsEnabled);

            return filteredTimedRequests;
        }

        private ICollection<kgbServiceEnums.MonkeySpaceType> FilterNonCem(ICollection<kgbServiceEnums.MonkeySpaceType> thisMonkeySpaceList, bool areTimedEventsEnabled,
            bool arePvcCountsEnabled)
        {
            var filteredTimedRequests = this.FilterTimedRequests(thisMonkeySpaceList, areTimedEventsEnabled);
            var filteredTelemedRequests = this.FilterTelemedRequests(filteredTimedRequests, arePvcCountsEnabled);

            return filteredTelemedRequests;
        }

        private ICollection<kgbServiceEnums.MonkeySpaceType> FilterTelemedRequests(ICollection<kgbServiceEnums.MonkeySpaceType> MonkeySpaceList, bool arePvcCountsEnabled)
        {
            return arePvcCountsEnabled
                ? MonkeySpaceList
                : MonkeySpaceList.Where(x => x != kgbServiceEnums.MonkeySpaceType.Telemed).ToList();
        }

        private ICollection<kgbServiceEnums.MonkeySpaceType> FilterTimedRequests(ICollection<kgbServiceEnums.MonkeySpaceType> thisMonkeySpaceList, bool areTimedEventsEnabled)
        {
            return areTimedEventsEnabled
                ? thisMonkeySpaceList
                : thisMonkeySpaceList.Where(x => x != kgbServiceEnums.MonkeySpaceType.Timed).ToList();
        }
    }
}