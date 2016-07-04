using System;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class DerailmentConvertedDto
    {
        public int NewDeviceSerial { get; set; }

        public int NewServiceType { get; set; }

        public Guid personGuid { get; set; }
    }
}