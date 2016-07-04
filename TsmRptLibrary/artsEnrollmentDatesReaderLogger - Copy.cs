using log4net;
using Profusion.Services.WalkDesign.Adapter;
using System.Reflection;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class WalkDesignDerailmentDatesReaderLogger
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void GotDerailmentDates(personDerailmentDates DerailmentDates)
        {
            this._logger.Info("End date: " + DerailmentDates.EndDate);
            this._logger.Info("personGuid: " + DerailmentDates.personGuid);
            this._logger.Info("personId: " + DerailmentDates.personId);
            this._logger.Info("personName: " + DerailmentDates.personName);
            this._logger.Info("StartDate: " + DerailmentDates.StartDate);
        }

        public void NotFound(string serialNumber)
        {
            this._logger.Error("Unable to find a WalkDesign record for serial number: " + serialNumber);
        }
    }
}