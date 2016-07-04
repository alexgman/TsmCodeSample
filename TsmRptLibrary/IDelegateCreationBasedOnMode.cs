using System.Collections.Generic;

namespace TsmRptLibrary
{
    internal interface IDelegateCreationBasedOnMode
    {
        ICollection<EventAutomationEntry> Start(ITag1RulesEngine tag1RulesEngine);
    }
}