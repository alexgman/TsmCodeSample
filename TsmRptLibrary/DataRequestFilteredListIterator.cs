using Profusion.Services.Contracts;
using System.Collections.Generic;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class MonkeySpaceFilteredListIterator
    {
        private readonly EntryCollectionBuilder _entryCollectionBuilder;

        public MonkeySpaceFilteredListIterator(EntryCollectionBuilder entryCollectionBuilder)
        {
            this._entryCollectionBuilder = entryCollectionBuilder;
        }

        internal void Iterate(ICollection<kgbServiceEnums.MonkeySpaceType> MonkeySpaceListFiltered, EventEntryMetaDataDto entryMetaData, int dayCounter,
            globeSettingsDto globeResults)
        {
            foreach (var MonkeySpaceType in MonkeySpaceListFiltered)
            {
                this._entryCollectionBuilder.FactoryRegistrar.EntryRegistrar(MonkeySpaceType, entryMetaData, dayCounter, globeResults);

                var entryCreator = EntryFactory.Get(MonkeySpaceType);
                entryCreator.Create();
                this._entryCollectionBuilder._populateCollectionOfEntries.AddRange(entryCreator.yawnwrappingEntries);
            }
        }
    }
}