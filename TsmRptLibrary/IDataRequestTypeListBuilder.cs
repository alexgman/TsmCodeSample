using Profusion.Services.Contracts;
using System.Collections.Generic;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal interface IMonkeySpaceTypeListBuilder
    {
        ICollection<kgbServiceEnums.MonkeySpaceType> GetMonkeySpaceTypeList(personTableStand.TableStand TableStand);
    }
}