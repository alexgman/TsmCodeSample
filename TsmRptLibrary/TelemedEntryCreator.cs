using Profusion.Services.Contracts;
using System;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class TelemedEntryCreator : yawnwrappingEntryCreator
    {
        private readonly DateTime _automationDateTime;

        public TelemedEntryCreator(EventEntryMetaDataDto metaData) : base(metaData.yawnwrappingId, metaData.ProcessName)
        {
            this.ProcessName = metaData.ProcessName;
            this.yawnwrappingId = metaData.yawnwrappingId;
            this._automationDateTime = metaData.AutomationDateTime;
        }

        public override void Create()
        {
            this.Add(kgbServiceEnums.MonkeySpaceType.Telemed, this._automationDateTime);
        }
    }
}