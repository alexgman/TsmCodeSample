using Revampness.Services.Contracts;
using Revampness.Services.device.Model;
using System;
using System.Collections.Generic;

namespace TsmRptLibrary
{
    internal interface IMinEntryCreator
    {
        ICollection<EventAutomationEntry> EventAutomationEntries { get; set; }

        void Add(EcgServiceEnums.DataRequestType dataRequestType, DateTime automationDateTime);

        void Create();
    }
}