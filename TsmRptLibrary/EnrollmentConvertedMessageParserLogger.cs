using log4net;
using System;
using System.Reflection;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class DerailmentConvertedMessageParserLogger
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Parse(int serviceType, int serialNumber, Guid ptGuid)
        {
            this._logger.Info("Service type: " + serviceType);
            this._logger.Info("Serial Number: " + serialNumber);
            this._logger.Info("person Guid: " + ptGuid);
        }
    }
}