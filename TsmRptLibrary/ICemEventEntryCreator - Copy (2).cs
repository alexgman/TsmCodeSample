using System.Collections.Generic;

namespace TsmRptLibrary
{
    internal interface ICemEventEntryCreator
    {
        ICollection<EventAutomationEntry> Create(ITag1RulesEngine tag1RulesEngine, ITimedEntryCreator timedEntryCreator);
    }
}