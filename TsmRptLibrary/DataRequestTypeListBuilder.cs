using Profusion.Services.Contracts;
using System.Collections.Generic;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class MonkeySpaceTypeListBuilder : IMonkeySpaceTypeListBuilder
    {
        public ICollection<kgbServiceEnums.MonkeySpaceType> GetMonkeySpaceTypeList(personTableStand.TableStand TableStand)
        {
            var requestList = new List<kgbServiceEnums.MonkeySpaceType>();

            if (TableStand == personTableStand.TableStand.Cem)
            {
                requestList.Add(kgbServiceEnums.MonkeySpaceType.Timed);
                return requestList;
            }

            requestList.Add(kgbServiceEnums.MonkeySpaceType.Timed);
            requestList.Add(kgbServiceEnums.MonkeySpaceType.Telemed);
            requestList.Add(kgbServiceEnums.MonkeySpaceType.MinimumHr);
            requestList.Add(kgbServiceEnums.MonkeySpaceType.MaximumHr);

            return requestList;
        }
    }
}