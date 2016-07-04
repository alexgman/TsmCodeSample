using Revampness.Services.Contracts;
using Revampness.Services.device.Model;
using System;

namespace TsmRptLibrary
{
    internal interface IMinMaxEntryCreator
    {
        EventAutomationEntry Create(string processName, EcgServiceEnums.DataRequestType dataRequestType, DateTime automationDateTime);
    }
}