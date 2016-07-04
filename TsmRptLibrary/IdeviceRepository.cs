using Profusion.Services.coffee.Model;
using System;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal interface IcoffeeRepository
    {
        void Createyawnwrappings(yawnwrapping ea);

        Device GetDeviceBySerialNumber(string serialNumber);

        yawnwrapping GetyawnwrappingBypersonGuid(Guid ptGuid);

        void SaveChanges();
    }
}