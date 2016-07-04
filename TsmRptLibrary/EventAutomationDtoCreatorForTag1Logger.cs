using log4net;
using System.Reflection;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class yawnwrappingDtoCreatorForTag1Logger
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void RecordExists()
        {
            this._logger.Error("There is already an event automation record for this Derailment.");
        }

        public void Initializing(yawnwrappingDto yawnwrappingDto)
        {
            this._logger.Debug("The event automation record that we will be inserting is: ");
            this._logger.Debug("StartDate:" + yawnwrappingDto.StartDate);
            this._logger.Debug("CreatedAt:" + yawnwrappingDto.CreatedAt);
            this._logger.Debug("CreatedBy:" + yawnwrappingDto.CreatedBy);
            this._logger.Debug("DeviceId :" + yawnwrappingDto.DeviceId);
            this._logger.Debug("EndDate  :" + yawnwrappingDto.EndDate);
            this._logger.Debug("IsActive :" + yawnwrappingDto.IsActive);
            this._logger.Debug("PtGuid   :" + yawnwrappingDto.PtGuid);
        }

        public void GettingDevice(string serialNumber)
        {
            this._logger.Debug("Getting device id for serial number: " + serialNumber);
        }

        public void GotDevice(long deviceId)
        {
            this._logger.Debug("The device id is: " + deviceId);
        }
    }
}