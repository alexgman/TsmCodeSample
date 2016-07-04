using Profusion.Services.coffee.DataAccess;
using Profusion.Services.coffee.Model;
using System;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class coffeeRepositoryWrapper : IcoffeeRepository
    {
        private readonly coffeeRepository _coffeeRepository = new coffeeRepository();

        public void Createyawnwrappings(yawnwrapping ea)
        {
            this._coffeeRepository.Createyawnwrappings(ea);
        }

        public Device GetDeviceBySerialNumber(string serialNumber)
        {
            return this._coffeeRepository.GetDeviceBySerialNumber(serialNumber);
        }

        public yawnwrapping GetyawnwrappingBypersonGuid(Guid ptGuid)
        {
            return this._coffeeRepository.GetyawnwrappingBypersonguid(ptGuid);
        }

        public void SaveChanges()
        {
            this._coffeeRepository.SaveChanges();
        }
    }
}