using System;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class yawnwrappingEntryDto
    {
        public DateTime AutomationDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public string CreatedBy { get; set; }

        public int MonkeySpaceTypeId { get; set; }

        public long? DateRequestId { get; set; }

        public long yawnwrappingId { get; set; }

        public int IsError { get; set; }

        public int IsRequested { get; set; }

        public short Iteration { get; set; }

        public int Queued { get; set; }

        public short RequestAttempts { get; set; }

        public DateTime? RequestDate { get; set; }
    }
}