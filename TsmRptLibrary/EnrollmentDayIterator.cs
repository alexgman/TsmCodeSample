using log4net;
using Revampness.Services.device.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading;

namespace Revampness.Services.device.ReportTablePreprocessor
{
    internal class EnrollmentDayIterator : IEnrollmentDayIterator
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected ILog GetLogger()
        {
            return LogManager.GetLogger(this.GetType().Assembly, this.GetType().Name + '.' + Thread.CurrentThread.ManagedThreadId);
        }

        public ICollection<EventAutomationEntry> GetAllEntries(ITag1RulesEngine tag1RulesEngine, IDelegateCreationBasedOnMode delegateCreationBasedOnMode)
        {
            var automationDate = tag1RulesEngine.Tag1Rules.PatientEnrollmentStart;
            var patientEnrollmentEndDate = tag1RulesEngine.Tag1Rules.PatientEnrollmentEnd;

            var entries = new List<EventAutomationEntry>();

            for (; automationDate < patientEnrollmentEndDate; automationDate = automationDate.AddDays(1))
            {
                IterateDataRequests(tag1RulesEngine, delegateCreationBasedOnMode, entries);
            }

            return entries;
        }

        private static void IterateDataRequests(ITag1RulesEngine tag1RulesEngine, IDelegateCreationBasedOnMode delegateCreationBasedOnMode, List<EventAutomationEntry> entries)
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

            tag1RulesEngine.Tag1Rules.DayCounter++;
        }
    }
}