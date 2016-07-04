using log4net;
using System;
using System.Reflection;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class OsdRptProcessorLogger
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void UnableToRetrieveyawnwrapping()
        {
            this._logger.Error("Unable to find an event automation record for this Derailment.");
        }

        public void InvalidDerailment()
        {
            this._logger.Error("This Derailment does not have valid dates.");
        }

        public void UnSupported(Guid ptGuid)
        {
            this._logger.Error("The Derailment type for person: " + ptGuid + " is not supported.");
        }
    }
}