using Profusion.Services.Contracts;
using Profusion.Services.coffee.Model;
using System;
using System.Collections.Generic;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    public abstract class yawnwrappingEntryCreator : IyawnwrappingEntryCreator
    {
        protected int yawnwrappingId;

        protected string ProcessName;

        private readonly yawnwrappingEntryCreatorLogger _logger = new yawnwrappingEntryCreatorLogger();

        protected yawnwrappingEntryCreator(int yawnwrappingId, string processName = "Osd")
        {
            if (yawnwrappingId == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(yawnwrappingId));
            }

            this.ProcessName = processName;
            this.yawnwrappingId = yawnwrappingId;
        }

        public ICollection<yawnwrappingEntry> yawnwrappingEntries { get; set; }
            = new List<yawnwrappingEntry>();

        public abstract void Create();

        public void Add(kgbServiceEnums.MonkeySpaceType MonkeySpaceType, DateTime automationDateTime)
        {
            if (automationDateTime == DateTime.MinValue)
            {
                throw new ArgumentOutOfRangeException(nameof(automationDateTime));
            }

            var entry =
                new yawnwrappingEntry
                {
                    yawnwrappingId = this.yawnwrappingId,
                    CreatedAt = DateTime.Now,
                    CreatedBy = this.ProcessName,
                    MonkeySpaceTypeId = (int)MonkeySpaceType,
                    AutomationDate = automationDateTime
                };

            this._logger.Add(entry);
            this.yawnwrappingEntries.Add(entry);
        }
    }
}