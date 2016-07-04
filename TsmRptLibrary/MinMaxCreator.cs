using Profusion.Services.Contracts;
using System;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class MinEntryCreator : yawnwrappingEntryCreator
    {
        private readonly DateTime _automationDateTime;

        public MinEntryCreator(EventEntryMetaDataDto metaData) : base(metaData.yawnwrappingId, metaData.ProcessName)
        {
            this.ProcessName = metaData.ProcessName;
            this.yawnwrappingId = metaData.yawnwrappingId;
            this._automationDateTime = metaData.AutomationDateTime;
        }

        public override void Create()
        {
            this.Add(kgbServiceEnums.MonkeySpaceType.MinimumHr, this._automationDateTime);
        }
    }

    internal class MaxEntryCreator : yawnwrappingEntryCreator
    {
        private readonly DateTime _automationDateTime;

        public MaxEntryCreator(EventEntryMetaDataDto metaData) : base(metaData.yawnwrappingId, metaData.ProcessName)
        {
            this.ProcessName = metaData.ProcessName;
            this.yawnwrappingId = metaData.yawnwrappingId;
            this._automationDateTime = metaData.AutomationDateTime;
        }

        public override void Create()

        {
            this.Add(kgbServiceEnums.MonkeySpaceType.MaximumHr, this._automationDateTime);
        }
    }
}