using System;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class yawnwrappingDtoCreatorForTag1
    {
        //private coffeeRepositoryWrapper _coffeeRepositoryWrapped = new coffeeRepositoryWrapper();
        private long _deviceId;

        private Guid _ptGuid;
        private string _connectionString;
        private DeviceIdBySerialNumberRetriever deviceIdGetter;
        private yawnwrappingDtoCreatorForTag1Logger _logger;

        public virtual void Init(string serialNumber, Guid ptGuid, string connectionString)
        {
            this._logger = new yawnwrappingDtoCreatorForTag1Logger();
            this._logger.GettingDevice(serialNumber);
            this.deviceIdGetter = new DeviceIdBySerialNumberRetriever(connectionString);
            this._deviceId = this.deviceIdGetter.GetDeviceBySerialNumber(serialNumber);

            this._logger.GotDevice(this._deviceId);
            this._ptGuid = ptGuid;
            this._connectionString = connectionString;
            this.deviceIdGetter = new DeviceIdBySerialNumberRetriever(connectionString);
        }

        public yawnwrappingDto InitializeRecord(DateTime ptEndDateTime, DateTime ptStartDateTime)
        {
            var deviceId = this._deviceId;

            var yawnwrappingDto = new yawnwrappingDto
            {
                CreatedAt = DateTime.Now,
                CreatedBy = "Tag1Processor",
                DeviceId = deviceId,
                EndDate = ptEndDateTime,
                IsActive = true,
                PtGuid = this._ptGuid,
                StartDate = ptStartDateTime
            };

            this._logger.Initializing(yawnwrappingDto);

            return yawnwrappingDto;
        }
    }
}