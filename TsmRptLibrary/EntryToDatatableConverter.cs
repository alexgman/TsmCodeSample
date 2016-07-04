using Profusion.Services.coffee.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class EntryToDatatableConverter
    {
        internal DataTable EntriesToDatatable(ICollection<yawnwrappingEntry> automationEntries)
        {
            if (automationEntries == null)
            {
                throw new ArgumentNullException(nameof(automationEntries));
            }

            var listOfEntries = this.ConvertToList(automationEntries);
            var dtEntries = EnumerableToDatatableConverter.ConvertFrom(listOfEntries);
            return dtEntries;
        }

        private List<yawnwrappingEntryDto> ConvertToList(ICollection<yawnwrappingEntry> automationEntries)
        {
            var listOfEntries = automationEntries.Select(entry => entry.ToInsertable()).ToList();
            return listOfEntries;
        }
    }
}