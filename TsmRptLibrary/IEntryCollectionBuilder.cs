using Profusion.Services.WalkDesign.Adapter;
using Profusion.Services.coffee.Model;
using System;
using System.Collections.Generic;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal interface IEntryCollectionBuilder
    {
        void Init();

        ICollection<yawnwrappingEntry> PopulateCollectionOfEntries(DateTime yawnwrappingEndDate, personDerailmentDates ptDerailmentDates,
            int yawnwrappingId, Guid ptGuid, personTableStand.TableStand ptMode, string OsdOrRpt);
    }
}