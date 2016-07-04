using System;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class EventEntryMetaDataCreator
    {
        private readonly EventEntryMetaDataCreatorLogger _logger = new EventEntryMetaDataCreatorLogger();

        public EventEntryMetaDataDto CreateEntryMetaData(DateTime automationDateTime, int yawnwrappingId, string processName = "Ponies")
        {
            if (automationDateTime == DateTime.MinValue)
            {
                throw new ArgumentOutOfRangeException(nameof(automationDateTime));
            }

            if (yawnwrappingId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(yawnwrappingId));
            }

            var entryMetaData = new EventEntryMetaDataDto
            {
                ProcessName = processName,
                AutomationDateTime = automationDateTime,
                yawnwrappingId = yawnwrappingId
            };

            //this._logger.Created(entryMetaData);

            return entryMetaData;
        }
    }
}