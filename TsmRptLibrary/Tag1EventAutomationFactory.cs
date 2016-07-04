using log4net;
using System;
using System.Reflection;
using System.Threading;

namespace TsmRptLibrary
{
    internal class Tag1EventAutomationFactory : ITag1EventAutomationFactory
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected ILog GetLogger()
        {
            return LogManager.GetLogger(this.GetType().Assembly, this.GetType().Name + '.' + Thread.CurrentThread.ManagedThreadId);
        }

        public string CreatedByProcessName { get; private set; } = string.Empty;

        public void Configure(IConfigHelper configHelper)
        {
            if (configHelper == null)
            {
                throw new ArgumentNullException(nameof(configHelper));
            }

            this.CreatedByProcessName = configHelper.CreatedByProcessName;
        }

        public void Start(ITag1RulesEngine tag1RulesEngine, IdeviceRepository deviceRepository)
        {
            tag1RulesEngine.UpdateDayCounter();

            if (!tag1RulesEngine.HasAutomations())
            {
                this.ProcessNewAutomation(tag1RulesEngine, deviceRepository);
            }
            else
            {
                if (!this.AreEnrollmentDatesCorrect(tag1RulesEngine))
                {
                    return;
                }

                this.ProcessExistingAutomation(tag1RulesEngine, deviceRepository);
            }
        }

        private void ProcessNewAutomation(ITag1RulesEngine tag1RulesEngine, IdeviceRepository deviceRepository)
        {
            var delegateEntryCreation = new DelegateCreationBasedOnMode(new NonCemEventEntryCreator(), new CemEventEntryCreator());
            tag1RulesEngine.EventAutomation = new EventAutomationCreator(new deviceRepositoryWrapper()).Create(tag1RulesEngine);
            tag1RulesEngine.EventAutomation.EventAutomationEntries = new EnrollmentDayIterator().GetAllEntries(tag1RulesEngine, delegateEntryCreation);
            deviceRepository.CreateEventAutomations(tag1RulesEngine.EventAutomation);
            deviceRepository.SaveChanges();
        }

        private void ProcessExistingAutomation(ITag1RulesEngine tag1RulesEngine, IdeviceRepository deviceRepository)
        {
            //we are not processing existing automation, will have to refactor this and get rid of it
            return;

            var delegateEntryCreation = new DelegateCreationBasedOnMode(new NonCemEventEntryCreator(), new CemEventEntryCreator());
            this.DeleteExistingEntries(tag1RulesEngine);

            tag1RulesEngine.EventAutomation.EventAutomationEntries.AddRange(new EnrollmentDayIterator().GetAllEntries(tag1RulesEngine, delegateEntryCreation));
            deviceRepository.SaveChanges();
        }

        private bool AreEnrollmentDatesCorrect(ITag1RulesEngine tag1RulesEngine)
        {
            if (tag1RulesEngine.DidEnrollmentDurationChange)
            {
                return false;
            }

            if (!tag1RulesEngine.IsEnrollmentExtended)
            {
                return false;
            }
            return true;
        }

        private void DeleteExistingEntries(ITag1RulesEngine tag1RulesEngine)
        {
            var serialNumber = tag1RulesEngine.Tag1Rules.SerialNumber;
            var patientGuid = tag1RulesEngine.Tag1Rules.PatientGuid;
            var deleteAutomationEntries = new DeleteEventAutomationEntries("connectionstring");
            deleteAutomationEntries.DeleteEntries(serialNumber, patientGuid);
        }
    }
}