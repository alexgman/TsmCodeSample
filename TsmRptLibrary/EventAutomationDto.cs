using System;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class yawnwrappingDto
    {
        public DateTime CreatedAt { get; set; }

        public string CreatedBy { get; set; }

        public long DeviceId { get; set; }

        public DateTime EndDate { get; set; }

        public long Id { get; set; }

        public bool IsActive { get; set; }

        public Guid PtGuid { get; set; }

        public DateTime StartDate { get; set; }
    }
}