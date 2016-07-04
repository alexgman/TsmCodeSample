using Profusion.Services.coffee.Model;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal static class yawnwrappingEntryExtensions
    {
        public static yawnwrappingEntryDto ToInsertable(this yawnwrappingEntry inputEntry)
        {
            var ouputEntry = new yawnwrappingEntryDto
            {
                AutomationDate = inputEntry.AutomationDate,
                CreatedAt = inputEntry.CreatedAt,
                CreatedBy = inputEntry.CreatedBy,
                MonkeySpaceTypeId = inputEntry.MonkeySpaceTypeId,
                DateRequestId = inputEntry.MonkeySpaceId < 1 ? null : inputEntry.MonkeySpaceId,
                IsError = inputEntry.IsError ? 1 : 0,
                Iteration = inputEntry.Iteration,
                IsRequested = inputEntry.IsRequested ? 1 : 0,
                Queued = inputEntry.Queued ? 1 : 0,
                RequestAttempts = inputEntry.RequestAttempts,
                RequestDate = inputEntry.RequestDate,
                yawnwrappingId = inputEntry.yawnwrappingId
            };

            return ouputEntry;
        }
    }
}