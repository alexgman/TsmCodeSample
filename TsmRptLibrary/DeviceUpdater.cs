using Magnum.Extensions;
using Profusion.Services.coffee.Model;
using System;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class DeviceUpdater
    {
        private readonly DeviceUpdaterLogger _logger = new DeviceUpdaterLogger();

        public void UpdateDeviceId(yawnwrapping yawnwrapping, string serialNumber, IcoffeeRepository coffeeRepository)
        {
            if (yawnwrapping == null)
            {
                throw new ArgumentNullException(nameof(yawnwrapping));
            }
            if (!serialNumber.IsNotEmpty())
            {
                throw new ArgumentNullException(nameof(serialNumber));
            }
            if (coffeeRepository == null)
            {
                throw new ArgumentNullException(nameof(coffeeRepository));
            }
            var deviceId = coffeeRepository.GetDeviceBySerialNumber(serialNumber).Id;
            this._logger.DeviceUpdated(yawnwrapping.DeviceId, deviceId);
            yawnwrapping.DeviceId = deviceId;
            coffeeRepository.SaveChanges();
        }
    }
}