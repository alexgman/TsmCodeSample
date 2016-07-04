using Revampness.Services.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace TsmRptLibrary
{
    internal static class DataRequestTypeCollectionExtensions
    {
        private static ICollection<EcgServiceEnums.DataRequestType> FilterTelemedRequests(this ICollection<EcgServiceEnums.DataRequestType> dataRequestList, bool arePvcCountsEnabled)
        {
            return arePvcCountsEnabled
                ? dataRequestList
                : dataRequestList.Where(x => x != EcgServiceEnums.DataRequestType.Telemed).ToList();
        }

        private static ICollection<EcgServiceEnums.DataRequestType> FilterTimedRequests(this
            ICollection<EcgServiceEnums.DataRequestType> thisDataRequestList, bool areTimedEventsEnabled)
        {
            return areTimedEventsEnabled
                ? thisDataRequestList
                : thisDataRequestList.Where(x => x != EcgServiceEnums.DataRequestType.Timed).ToList();
        }

        public static ICollection<EcgServiceEnums.DataRequestType> FilterCem(this ICollection<EcgServiceEnums.DataRequestType> thisDataRequestList, bool areTimedEventsEnabled)
        {
            var filteredTimedRequests = thisDataRequestList.FilterTimedRequests(areTimedEventsEnabled);

            return filteredTimedRequests;
        }

        public static ICollection<EcgServiceEnums.DataRequestType> FilterNonCem(this ICollection<EcgServiceEnums.DataRequestType> thisDataRequestList, bool areTimedEventsEnabled, bool arePvcCountsEnabled)
        {
            var filteredTimedRequests = thisDataRequestList.FilterTimedRequests(areTimedEventsEnabled);
            var filteredTelemedRequests = filteredTimedRequests.FilterTelemedRequests(arePvcCountsEnabled);

            return filteredTelemedRequests;
        }
    }
}