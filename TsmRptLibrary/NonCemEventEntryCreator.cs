using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace TsmRptLibrary
{
    internal class NonCemEventEntryCreator : INonCemEventEntryCreator
    {
        private IMinMaxEntryCreator _minmaxcEntryCreator;
        private ITelemdEntryCreator _telemeEntryCreator;
        private ITimedEntryCreator _timedEntryCreator;

        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected ILog GetLogger()
        {
            return LogManager.GetLogger(this.GetType().Assembly, this.GetType().Name + '.' + Thread.CurrentThread.ManagedThreadId);
        }

        public ICollection<EventAutomationEntry> Create(ITag1RulesEngine tag1RulesEngine, IEventAutomationEntryCreator createAutomationEntry)
        {
            var eventAutomationEntry = new List<EventAutomationEntry>();

            switch (tag1RulesEngine.Tag1Rules.DataRequestType)
            {
                case EcgServiceEnums.DataRequestType.Telemed:
                    if (tag1RulesEngine.Tag1Rules.AtlasEnabledEctopyCounts)
                    {
                        this._telemeEntryCreator = new TelemdEntryCreator(createAutomationEntry);
                        eventAutomationEntry.Add(this._telemeEntryCreator.Create("process", EcgServiceEnums.DataRequestType.Telemed, tag1RulesEngine.Tag1Rules.EventAutomation.StartDate));
                    }
                    break;

                case EcgServiceEnums.DataRequestType.Timed:
                    if (tag1RulesEngine.AreTimedEventsEnabled())
                    {
                        this._timedEntryCreator = new TimedEntryCreator(createAutomationEntry, tag1RulesEngine);
                        return this._timedEntryCreator.Create("process", EcgServiceEnums.DataRequestType.Timed, tag1RulesEngine.Tag1Rules.EventAutomation.StartDate);
                    }
                    break;

                case EcgServiceEnums.DataRequestType.MinimumHr:
                    this._minmaxcEntryCreator = new MinMaxEntryCreator(createAutomationEntry, EcgServiceEnums.DataRequestType.MinimumHr);
                    eventAutomationEntry.Add(this._minmaxcEntryCreator.Create("process", EcgServiceEnums.DataRequestType.MinimumHr, tag1RulesEngine.Tag1Rules.EventAutomation.StartDate));
                    break;

                case EcgServiceEnums.DataRequestType.MaximumHr:
                    this._minmaxcEntryCreator = new MinMaxEntryCreator(createAutomationEntry, EcgServiceEnums.DataRequestType.MaximumHr);
                    eventAutomationEntry.Add(this._minmaxcEntryCreator.Create("process", EcgServiceEnums.DataRequestType.MaximumHr, tag1RulesEngine.Tag1Rules.EventAutomation.StartDate));
                    break;

                case EcgServiceEnums.DataRequestType.Via:
                    break;

                case EcgServiceEnums.DataRequestType.ProactiveMinimumHr:
                    break;

                case EcgServiceEnums.DataRequestType.ProactiveMaximumHr:
                    break;

                case EcgServiceEnums.DataRequestType.ShortInterval:
                    break;

                case EcgServiceEnums.DataRequestType.Generic:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return eventAutomationEntry;
        }
    }
}