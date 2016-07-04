using Profusion.Services.coffee.Model;
using System;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal interface IStartupAutomation
    {
        DateTime LastAttemptDate { get; set; }
        DateTime OriginalQueueDate { get; set; }
        int RetryCounter { get; set; }
        EventLog Tze { get; set; }
    }
}