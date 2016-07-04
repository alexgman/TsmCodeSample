using System.Collections.Generic;
using System.Data;
using Profusion.Services.coffee.Model;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal interface IInsertyawnwrappingEntry
    {
        void InsertEntriesFromDatatable(DataTable dtEntries, string sqlCommandString = "InsertyawnwrappingEntries",
            string tableVariable = "@TableVariable", string tableType = "yawnwrappingEntryTableType");

        void InsertChildEntries(ICollection<yawnwrappingEntry> entries, string sqlCommandString = "InsertyawnwrappingEntriesToParent");
    }
}