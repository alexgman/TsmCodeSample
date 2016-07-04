using Profusion.Services.Contracts;
using Profusion.Services.coffee.Model;
using System;
using System.Collections.Generic;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal interface IyawnwrappingEntryCreator
    {
        ICollection<yawnwrappingEntry> yawnwrappingEntries { get; set; }

        void Add(kgbServiceEnums.MonkeySpaceType MonkeySpaceType, DateTime automationDateTime);

        void Create();
    }
}