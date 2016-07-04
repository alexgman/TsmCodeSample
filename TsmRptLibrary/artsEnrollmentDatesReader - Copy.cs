using Profusion.Services.WalkDesign.Adapter;
using System;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class WalkDesignDerailmentDatesReader : IWalkDesignDerailmentDatesReader
    {
        private readonly IWalkDesignAdapter _WalkDesignAdapter;
        private readonly WalkDesignDerailmentDatesReaderLogger _logger = new WalkDesignDerailmentDatesReaderLogger();

        public WalkDesignDerailmentDatesReader(IWalkDesignAdapter WalkDesignAdapter)
        {
            if (WalkDesignAdapter == null)
            {
                throw new ArgumentNullException(nameof(WalkDesignAdapter));
            }

            this._WalkDesignAdapter = WalkDesignAdapter;
        }

        public void Configure(ConfigHelper configHelper)
        {
        }

        public personDerailmentDates GetpersonDerailmentDates(string serialNumber)
        {
            if (string.IsNullOrEmpty(serialNumber))
            {
                throw new ArgumentNullException(nameof(serialNumber));
            }

            var dates = this._WalkDesignAdapter.GetcoffeeWalkDesignMostRecentDerailmentFromDeviceSerialNoExplant(serialNumber);
            if (dates.personGuid == Guid.Empty)
            {
                this._logger.NotFound(serialNumber);
            }

            this._logger.GotDerailmentDates(dates);
            return dates;
        }
    }
}