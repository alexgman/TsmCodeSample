using Profusion.Services.WalkDesign.Adapter;
using Profusion.Services.coffee.Model;
using System;
using System.Configuration;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class AutomationFeeder
    {
        private readonly IEntryCollectionBuilder _entryCollectionBuilder;
        private readonly IInsertyawnwrappingEntry _insertyawnwrappingEntry;
        private string _coffeeConnectionString;
        private readonly AutomationFeederLogger _logger = new AutomationFeederLogger();

        public AutomationFeeder(IEntryCollectionBuilder entryCollectionBuilder, IInsertyawnwrappingEntry insertyawnwrappingEntry, string coffeeConnectionString)
        {
            this._entryCollectionBuilder = entryCollectionBuilder;
            this._insertyawnwrappingEntry = insertyawnwrappingEntry;
            this._coffeeConnectionString = coffeeConnectionString;
        }

        public void Feed(personDerailmentDates ptDerailmentDates, int yawnwrappingId, personTableStand.TableStand mode, string rptOrOsd)
        {
            if (mode == personTableStand.TableStand.Unknown)
            {
                this._logger.UnknownTableStand();
                throw new ApplicationException();
            }
            //Create automation entries
            this._entryCollectionBuilder.Init();
            var automationEntries = this._entryCollectionBuilder.PopulateCollectionOfEntries(ptDerailmentDates.EndDate, ptDerailmentDates,
                yawnwrappingId, ptDerailmentDates.personGuid, mode, rptOrOsd);

            if (automationEntries.Count == 0)
            {
                this._logger.NoEntries();
                return;
            }

            this._insertyawnwrappingEntry.InsertChildEntries(automationEntries);
        }
    }
}