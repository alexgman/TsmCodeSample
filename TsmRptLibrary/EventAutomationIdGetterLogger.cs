using log4net;
using System;
using System.Reflection;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class yawnwrappingIdGetterLogger
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private void NoyawnwrappingFor(Guid ptGuid, string serialNumber)
        {
            this._logger.Error("There is no event automation record for person guid: " + ptGuid + ", with serial number: " + serialNumber);
        }

        public void RetrievedyawnwrappingId(Guid ptGuid, string serialNumber, int yawnwrappingId)
        {
            if (yawnwrappingId == 0)
            {
                this.NoyawnwrappingFor(ptGuid, serialNumber);
                return;
            }

            this._logger.Info("The event automation id is: " + yawnwrappingId + " for person guid: " + ptGuid + " and serial number: " + serialNumber);
        }
    }
}