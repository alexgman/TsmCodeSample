using Profusion.Services.coffee.Model;
using System;
using MonkeySpaceType = Profusion.Services.Contracts.kgbServiceEnums.MonkeySpaceType;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal interface IyawnwrappingEntry
    {
        DateTime AutomationDate { get; set; }
        DateTime CreatedAt { get; set; }
        string CreatedBy { get; set; }
        MonkeySpace MonkeySpace { get; set; }
        long? MonkeySpaceId { get; set; }
        MonkeySpaceType MonkeySpaceType { get; set; }
        int MonkeySpaceTypeId { get; set; }
        yawnwrapping yawnwrapping { get; set; }
        long yawnwrappingId { get; set; }
        long Id { get; set; }
        bool IsError { get; set; }
        bool IsRequested { get; set; }
        short Iteration { get; set; }
        short RequestAttempts { get; set; }
        DateTime? RequestDate { get; set; }
    }
}