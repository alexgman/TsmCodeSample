using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace TsmRptLibrary
{
    internal class CemEventEntryCreator : ICemEventEntryCreator
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected ILog GetLogger()
        {
            return LogManager.GetLogger(this.GetType().Assembly, this.GetType().Name + '.' + Thread.CurrentThread.ManagedThreadId);
        }

        public ICollection<EventAutomationEntry> Create(ITag1RulesEngine tag1RulesEngine, ITimedEntryCreator timedEntryCreator)
        {
            return timedEntryCreator.Create("process", EcgServiceEnums.DataRequestType.Timed, tag1RulesEngine.Tag1Rules.EventAutomation.StartDate);
        }
    }
}