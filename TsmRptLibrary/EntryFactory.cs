using Profusion.Services.Contracts;
using System;
using System.Collections.Generic;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal static class EntryFactory
    {
        private static readonly Dictionary<kgbServiceEnums.MonkeySpaceType, Func<yawnwrappingEntryCreator>>
            Registry = new Dictionary<kgbServiceEnums.MonkeySpaceType, Func<yawnwrappingEntryCreator>>();

        public static yawnwrappingEntryCreator Get(kgbServiceEnums.MonkeySpaceType MonkeySpaceType)
        {
            return Registry[MonkeySpaceType].Invoke();
        }

        public static void Register(kgbServiceEnums.MonkeySpaceType MonkeySpaceType, Func<yawnwrappingEntryCreator> factory)
        {
            Registry[MonkeySpaceType] = factory;
        }
    }
}