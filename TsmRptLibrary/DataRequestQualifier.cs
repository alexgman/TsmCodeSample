using Revampness.Services.device.Model;
using System.Collections.Generic;

namespace TsmRptLibrary
{
    internal class DataRequestQualifier
    {
        public ICollection<EventAutomationEntry> GetAllEntries(ITag1RulesEngine tag1RulesEngine, IDelegateCreationBasedOnMode delegateCreationBasedOnMode)
        {
            var automationDate = tag1RulesEngine.Tag1Rules.PatientEnrollmentStart;
            var patientEnrollmentEndDate = tag1RulesEngine.Tag1Rules.PatientEnrollmentEnd;
            var entries = new List<EventAutomationEntry>();
            for (; automationDate < patientEnrollmentEndDate; automationDate = automationDate.AddDays(1))
            {
                this.Qualify(tag1RulesEngine, delegateCreationBasedOnMode, entries);

                tag1RulesEngine.Tag1Rules.DayCounter++;
            }

            return entries;
        }

        private void Qualify(ITag1RulesEngine tag1RulesEngine, IDelegateCreationBasedOnMode delegateCreationBasedOnMode, List<EventAutomationEntry> entries)
        {
            foreach (var dataRequestType in tag1RulesEngine.DataRequestTypes)
            {
                if (tag1RulesEngine.IgnoreCurrentDataRequest(dataRequestType))
                {
                    continue;
                }

                tag1RulesEngine.Tag1Rules.DataRequestType = dataRequestType;
                entries.AddRange(delegateCreationBasedOnMode.Start(tag1RulesEngine));
            }
        }
    }
}