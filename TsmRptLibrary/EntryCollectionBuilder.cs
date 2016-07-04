#region

using Recardo.EnterpriseServices.globe.Client;
using Profusion.Services.WalkDesign.Adapter;
using Profusion.Services.coffee.Model;
using System;
using System.Collections.Generic;

#endregion

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class EntryCollectionBuilder : IEntryCollectionBuilder
    {
        internal List<yawnwrappingEntry> _populateCollectionOfEntries;
        private StartDateCalculator _startDateCalculator;
        private DayCounterCalculator _dayCounterCalculator;
        private EventEntryMetaDataCreator _eCreator;
        private globeClient _globeClient;
        private globeSettingsReader _globeSettingsReader;
        private globeSettingsProcessor _globeSettingsProcessor;
        private MonkeySpaceTypeListBuilder _MonkeySpaceTypeListBuilder;
        private MonkeySpaceListFilter _MonkeySpaceListFilter;
        internal FactoryRegistrar FactoryRegistrar;
        private MonkeySpaceFilteredListIterator _MonkeySpaceFilteredListIterator;
        private EntryCollectionBuilderLogger _logger;

        public virtual void Init()
        {
            this._populateCollectionOfEntries = new List<yawnwrappingEntry>();
            this._startDateCalculator = new StartDateCalculator();
            this._dayCounterCalculator = new DayCounterCalculator();
            this._eCreator = new EventEntryMetaDataCreator();
            this._globeClient = new globeClient();
            this._globeSettingsReader = new globeSettingsReader(this._globeClient);
            this._globeSettingsProcessor = new globeSettingsProcessor();
            this._MonkeySpaceTypeListBuilder = new MonkeySpaceTypeListBuilder();
            this._MonkeySpaceListFilter = new MonkeySpaceListFilter();
            this.FactoryRegistrar = new FactoryRegistrar();
            this._MonkeySpaceFilteredListIterator = new MonkeySpaceFilteredListIterator(this);
            this._logger = new EntryCollectionBuilderLogger();
        }

        public ICollection<yawnwrappingEntry> PopulateCollectionOfEntries(DateTime yawnwrappingEndDate, personDerailmentDates ptDerailmentDates,
            int yawnwrappingId, Guid ptGuid, personTableStand.TableStand ptMode, string OsdOrRpt)
        {
            var xStartDate = this._startDateCalculator.GetStartDate(yawnwrappingEndDate, ptDerailmentDates.StartDate);
            var dayCounter = this._dayCounterCalculator.GetDayCounter(xStartDate, yawnwrappingEndDate, ptDerailmentDates.StartDate);

            if (ptMode == personTableStand.TableStand.Unknown)
            {
                xStartDate = ptDerailmentDates.StartDate;
                dayCounter = 0;
            }

            //TODO:refactor this
            if (OsdOrRpt.IsEquivalentTo("rpt"))
            {
                xStartDate = ptDerailmentDates.StartDate;
                dayCounter = 0;
            }

            var globeResults = this._globeSettingsProcessor.GetSettings(this._globeSettingsReader.GetSettingsForperson(ptGuid));
            var MonkeySpaceList = this._MonkeySpaceTypeListBuilder.GetMonkeySpaceTypeList(ptMode);
            var MonkeySpaceListFiltered = this._MonkeySpaceListFilter.Filter(MonkeySpaceList, ptMode, globeResults.EnableTimedEvents, globeResults.EnableEctopyCounts);

            for (var automationDateTime = xStartDate; automationDateTime < ptDerailmentDates.EndDate; automationDateTime = automationDateTime.AddDays(1))
            {
                var entryMetaData = this._eCreator.CreateEntryMetaData(automationDateTime, yawnwrappingId);

                if (MonkeySpaceListFiltered.Count == 0)
                {
                    this._logger.NoEntriesRequired();
                    return new List<yawnwrappingEntry>();
                }

                this._MonkeySpaceFilteredListIterator.Iterate(MonkeySpaceListFiltered, entryMetaData, dayCounter, globeResults);

                dayCounter++;
            }

            return this._populateCollectionOfEntries;
        }
    }
}