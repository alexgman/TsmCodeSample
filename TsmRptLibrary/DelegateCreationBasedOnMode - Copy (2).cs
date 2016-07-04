using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace TsmRptLibrary
{
    internal class DelegateCreationBasedOnMode : IDelegateCreationBasedOnMode
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected ILog GetLogger()
        {
            return LogManager.GetLogger(this.GetType().Assembly, this.GetType().Name + '.' + Thread.CurrentThread.ManagedThreadId);
        }

        public DelegateCreationBasedOnMode(INonCemEventEntryCreator nonCemEventEntryCreator, ICemEventEntryCreator cemEventEntryCreator)
        {
            this._nonCemEventEntryCreator = nonCemEventEntryCreator;
            this._cemEventEntryCreator = cemEventEntryCreator;
        }

        public ICollection<EventAutomationEntry> Start(ITag1RulesEngine tag1RulesEngine)
        {
            if (tag1RulesEngine.Tag1Rules.PatientServiceMode == PatientServiceMode.ServiceMode.Cem)
            {
                return this._cemEventEntryCreator.Create(tag1RulesEngine, new TimedEntryCreator(new EventAutomationEntryCreator(), tag1RulesEngine));
            }

            return this._nonCemEventEntryCreator.Create(tag1RulesEngine, new EventAutomationEntryCreator());
        }

        private readonly ICemEventEntryCreator _cemEventEntryCreator;
        private readonly INonCemEventEntryCreator _nonCemEventEntryCreator;
    }
}