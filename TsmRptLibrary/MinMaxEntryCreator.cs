using log4net;
using Revampness.Services.Contracts;
using Revampness.Services.device.Model;
using System;
using System.Reflection;
using System.Threading;

namespace TsmRptLibrary
{
    internal class MinMaxEntryCreator : IMinMax, IMinMaxEntryCreator
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected ILog GetLogger()
        {
            return LogManager.GetLogger(this.GetType().Assembly, this.GetType().Name + '.' + Thread.CurrentThread.ManagedThreadId);
        }

        private readonly IEventAutomationEntryCreator _createAutomationEntry;
        private readonly EcgServiceEnums.DataRequestType _dataRequestType;

        public MinMaxEntryCreator(IEventAutomationEntryCreator createAutomationEntry, EcgServiceEnums.DataRequestType dataRequestType)
        {
            this._createAutomationEntry = createAutomationEntry;
            this._dataRequestType = dataRequestType;
        }

        public EventAutomationEntry Create(string processName, EcgServiceEnums.DataRequestType dataRequestType, DateTime automationDateTime)

        {
            return this._createAutomationEntry.Create(processName, dataRequestType, automationDateTime);
        }
    }
}