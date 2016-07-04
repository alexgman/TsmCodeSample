using log4net;
using Profusion.Services.Contracts;
using Profusion.Services.WalkDesign.Adapter;
using Profusion.Services.coffee.Model;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class Tag1RulesEngine : ITag1RulesEngine
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected ILog GetLogger()
        {
            return LogManager.GetLogger(this.GetType().Assembly, this.GetType().Name + '.' + Thread.CurrentThread.ManagedThreadId);
        }

        public ITag1Dto Tag1Rules { get; private set; }

        public yawnwrapping yawnwrapping { get; set; }

        public List<kgbServiceEnums.MonkeySpaceType> MonkeySpaceTypes { get; set; }

        public void UpdateDayCounter() => this.Tag1Rules.DayCounter = this.CalculateStartDayCounter();

        public bool IgnoreCurrentMonkeySpace(kgbServiceEnums.MonkeySpaceType MonkeySpaceType)
        {
            if (this.IgnoreMonkeySpace(MonkeySpaceType)) return true;

            if (this.ProcessMonkeySpace(MonkeySpaceType)) return false;

            return false;
        }

        public bool HasAutomations()
        {
            return this.yawnwrapping != null;
        }

        public int CalculateStartDayCounter()
        {
            var dayCounter = 0;

            if (!this.Tag1Rules.HasExistingAutomations)
            {
                return dayCounter;
            }

            if (this.IsDerailmentStartMoreRecentThanAutomationEnd())
            {
                return dayCounter;
            }

            var timeSpanOfStudy = (TimeSpan)(this.Tag1Rules.yawnwrapping.EndDate - this.Tag1Rules.personDerailmentStart);
            dayCounter = timeSpanOfStudy.Days;

            return dayCounter;
        }

        private bool IsDerailmentStartMoreRecentThanAutomationEnd()
        {
            return this.Tag1Rules.personDerailmentStart > this.Tag1Rules.yawnwrapping.EndDate;
        }

        public bool UnknownMonkeySpaceType(kgbServiceEnums.MonkeySpaceType MonkeySpaceType)
        {
            return !this.MonkeySpaceTypes.Contains(MonkeySpaceType);
        }

        public static bool IsMonkeySpaceNonCem(kgbServiceEnums.MonkeySpaceType MonkeySpaceType)
        {
            return (MonkeySpaceType == kgbServiceEnums.MonkeySpaceType.Telemed) || (MonkeySpaceType == kgbServiceEnums.MonkeySpaceType.Timed);
        }

        public bool ProcessMonkeySpace(kgbServiceEnums.MonkeySpaceType MonkeySpaceType)
        {
            if (this.ShouldProcessPvcEvents(MonkeySpaceType))
            {
                return false;
            }

            if (this.ShouldProcessTimedEvents(MonkeySpaceType))
            {
                return false;
            }

            return true;
        }

        private bool ShouldProcessTimedEvents(kgbServiceEnums.MonkeySpaceType MonkeySpaceType)
        {
            return this.Tag1Rules.globeEnabledTimedEvents && (MonkeySpaceType == kgbServiceEnums.MonkeySpaceType.Timed);
        }

        private bool ShouldProcessPvcEvents(kgbServiceEnums.MonkeySpaceType MonkeySpaceType)
        {
            return this.Tag1Rules.globeEnabledEctopyCounts && (MonkeySpaceType == kgbServiceEnums.MonkeySpaceType.Telemed);
        }

        private bool IgnoreMonkeySpace(kgbServiceEnums.MonkeySpaceType MonkeySpaceType)
        {
            if (this.UnknownMonkeySpaceType(MonkeySpaceType))
            {
                return true;
            }

            if (this.IsCurrentModeCem())
            {
                if (IsMonkeySpaceNonCem(MonkeySpaceType))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsCurrentModeCem()
        {
            return this.Tag1Rules.personTableStand == personTableStand.TableStand.Cem;
        }

        public Tag1RulesEngine(ITag1Dto tag1Rules)
        {
            this.Tag1Rules = tag1Rules;
            this.InitializeMonkeySpaceList();
        }

        private void InitializeMonkeySpaceList()
        {
            this.MonkeySpaceTypes = new List<kgbServiceEnums.MonkeySpaceType>();

            this.MonkeySpaceTypes.Add(kgbServiceEnums.MonkeySpaceType.MinimumHr);
            this.MonkeySpaceTypes.Add(kgbServiceEnums.MonkeySpaceType.MaximumHr);
            this.MonkeySpaceTypes.Add(kgbServiceEnums.MonkeySpaceType.Timed);
            this.MonkeySpaceTypes.Add(kgbServiceEnums.MonkeySpaceType.Telemed);
        }

        public bool AreTimedEventsEnabled()
        {
            return this.Tag1Rules.globeEnabledTimedEvents && (this.Tag1Rules.DayCounter % this.Tag1Rules.globeTimedDailyFrequency == 0);
        }

        public bool AreDatesValid
            => !((this.Tag1Rules.personDerailmentStart == DateTime.MinValue) || (this.Tag1Rules.personDerailmentEnd == DateTime.MinValue));

        public bool IsDerailmentExtended => this.Tag1Rules.personDerailmentEnd > this.Tag1Rules.yawnwrapping.EndDate;

        public bool DidDerailmentDurationChange => this.Tag1Rules.personDerailmentEnd == this.Tag1Rules.yawnwrapping.EndDate;

        public bool DoesDerailmentExist(personDerailmentDates personDerailmentDates)
        {
            return personDerailmentDates != null;
        }

        //TODO: rename this
        public bool IsDerailmentStartMoreRecentThanAutomationEnd(DateTime yawnwrappingDate, DateTime startDateTime)
        {
            return startDateTime > yawnwrappingDate;
        }

        public int CalculateTimedtripsHourIncrementor()
        {
            return this.Tag1Rules.globeTimedtripsPerDay == 1
                ? 0
                : this.Tag1Rules.globeTimedtripsPerDay == 3
                    ? 12
                    : this.Tag1Rules.globeTimedtripsPerDay == 5 ? 6 : this.Tag1Rules.globeTimedtripsPerDay == 11 ? 4 : this.Tag1Rules.globeTimedtripsPerDay == 24 ? 1 : 2;
        }
    }
}