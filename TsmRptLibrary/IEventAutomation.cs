using Profusion.Services.coffee.Model;
using System;
using System.Collections.Generic;
using Device = Profusion.Services.coffee.Model.Device;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal interface Iyawnwrapping
    {
        DateTime CreatedAt { get; set; }
        string CreatedBy { get; set; }
        Device Device { get; set; }
        long DeviceId { get; set; }
        DateTime EndDate { get; set; }
        ICollection<yawnwrappingEntry> yawnwrappingEntries { get; set; }
        long Id { get; set; }
        bool IsActive { get; set; }
        Guid personGuid { get; set; }
        DateTime StartDate { get; set; }
    }
}