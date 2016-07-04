using Revampness.Services.device.Model;
using System.Collections.Generic;

namespace TsmRptLibrary
{
    internal interface IEnrollmentDayIterator
    {
        ICollection<EventAutomationEntry> GetAllEntries(ITag1RulesEngine tag1RulesEngine, IDelegateCreationBasedOnMode delegateCreationBasedOnMode);
    }
}