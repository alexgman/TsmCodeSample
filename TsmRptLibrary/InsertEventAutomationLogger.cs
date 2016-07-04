using log4net;
using System.Reflection;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class InsertyawnwrappingLogger
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Inserting(yawnwrappingDto yawnwrappingDto)
        {
            this._logger.Info("person Guid: " + yawnwrappingDto.PtGuid);
            this._logger.Info("Device id: " + yawnwrappingDto.DeviceId);
            this._logger.Info("Start date: " + yawnwrappingDto.StartDate);
            this._logger.Info("End date: " + yawnwrappingDto.EndDate);
            this._logger.Info("Created at: " + yawnwrappingDto.CreatedAt);
            this._logger.Info("Created by: " + yawnwrappingDto.CreatedBy);
            this._logger.Info("Is active: " + yawnwrappingDto.IsActive);
        }

        public void Inserted(int id)
        {
            this._logger.Info("Returned Id: " + id);
        }
    }
}