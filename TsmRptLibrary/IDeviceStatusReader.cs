using Profusion.Services.coffee.OsdRptLibrary.EppWebService;
using System;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal interface IDeviceStatusReader
    {
        DeviceStatus GetpersonDeviceStatus(Guid ptGuid);
    }
}