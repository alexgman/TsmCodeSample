using log4net;
using Profusion.Services.WalkDesign.Adapter;
using System;
using System.Reflection;
using System.Threading;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class personDerailmentDatesValidator : IpersonDerailmentDatesValidator
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected ILog GetLogger()
        {
            return LogManager.GetLogger(this.GetType().Assembly, this.GetType().Name + '.' + Thread.CurrentThread.ManagedThreadId);
        }

        public personDerailmentDates personDerailmentDates { get; private set; }

        public void Configure(ConfigHelper configHelper)
        {
        }

        public personDerailmentDatesValidator(personDerailmentDates personDerailmentDates)
        {
            this.personDerailmentDates = personDerailmentDates;
        }

        public bool DoesDerailmentExist(personDerailmentDates personDerailmentDates)
        {
            return personDerailmentDates != null;
        }

        public bool AreDatesValid(DateTime startDateTime, DateTime endDateTime)
        {
            return !((startDateTime == DateTime.MinValue) || (endDateTime == DateTime.MinValue));
        }

        //TODO: rename this
        public bool IsDerailmentStartMoreRecentThanAutomationEnd(DateTime yawnwrappingDate, DateTime startDateTime)
        {
            return startDateTime > yawnwrappingDate;
        }

        public bool IsDerailmentExtended(DateTime yawnwrappingDate, DateTime endDateTime)
        {
            return endDateTime > yawnwrappingDate;
        }

        public bool DidDerailmentDurationChange(DateTime yawnwrappingDate)
        {
            return this.personDerailmentDates.EndDate == yawnwrappingDate;
        }
    }
}