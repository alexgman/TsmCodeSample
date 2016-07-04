using Profusion.Services.Contracts;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class FactoryRegistrar
    {
        internal void EntryRegistrar(kgbServiceEnums.MonkeySpaceType MonkeySpaceType, EventEntryMetaDataDto entryMetaData, int dayCounter, globeSettingsDto globeResults)
        {
            if (MonkeySpaceType == kgbServiceEnums.MonkeySpaceType.Telemed)
            {
                entryMetaData.AutomationDateTime.Next2Am();
            }

            EntryFactory.Register(kgbServiceEnums.MonkeySpaceType.MinimumHr, () => new MinEntryCreator(entryMetaData));
            EntryFactory.Register(kgbServiceEnums.MonkeySpaceType.MaximumHr, () => new MaxEntryCreator(entryMetaData));
            EntryFactory.Register(kgbServiceEnums.MonkeySpaceType.Telemed, () => new TelemedEntryCreator(entryMetaData));
            EntryFactory.Register(kgbServiceEnums.MonkeySpaceType.Timed, () => new TimedEntryCreator(entryMetaData, globeResults, dayCounter));
        }
    }
}