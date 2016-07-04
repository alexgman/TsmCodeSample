using System;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal interface IDeleteyawnwrappingEntries
    {
        void DeleteAllChildEntries(int yawnwrappingId, string sproc = "spDeleteyawnwrappingEntries");

        void DeleteAllNonTimedEntries(string serialNumber, Guid ptGuid, string sproc = "spDeleteNonTimedyawnwrappingEntries");
    }
}