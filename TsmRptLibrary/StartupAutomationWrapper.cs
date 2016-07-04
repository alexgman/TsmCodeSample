using Profusion.Services.coffee.Model;
using System;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class StartupAutomationWrapper : IStartupAutomation
    {
        public virtual DateTime LastAttemptDate { get; set; }

        public DateTime OriginalQueueDate { get; set; }

        public int RetryCounter { get; set; }

        public EventLog Tze { get; set; }
    }
}