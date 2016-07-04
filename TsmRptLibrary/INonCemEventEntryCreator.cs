using System.Collections.Generic;

namespace TsmRptLibrary
{
    internal interface INonCemEventEntryCreator
    {
        ICollection<EventAutomationEntry> Create(ITag1RulesEngine tag1RulesEngine, IEventAutomationEntryCreator createAutomationEntry);
    }
}