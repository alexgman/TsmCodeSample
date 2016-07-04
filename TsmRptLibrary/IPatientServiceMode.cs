using System;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal interface IpersonTableStand
    {
        personTableStand.TableStand GetTableStand(IDeviceStatusReader deviceStatusReader, Guid ptGuid);
    }
}