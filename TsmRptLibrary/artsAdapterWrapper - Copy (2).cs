using Profusion.Services.WalkDesign.Adapter;
using System;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class WalkDesignAdapterWrapper : IWalkDesignAdapter
    {
        public personDerailmentDates GetcoffeeWalkDesignMostRecentDerailmentFromDeviceSerialNoExplant(string serialNumber)
        {
            if (!serialNumber.IsInt())
            {
                this._logger.SerialIsNotNumeric(serialNumber);
            }

            if (string.IsNullOrEmpty(serialNumber))
            {
                this._logger.SerialIsEmpty();
                throw new ArgumentNullException(nameof(serialNumber));
            }

            var personDerailmentDatesInfo = this._adapter.GetcoffeeWalkDesignMostRecentDerailmentFromDeviceSerialNoExplant(serialNumber);
            this._logger.GotDerailmentDates(personDerailmentDatesInfo);
            return personDerailmentDatesInfo;
        }

        private readonly WalkDesignAdapter _adapter = new WalkDesignAdapter();
        private readonly WalkDesignAdapterWrapperLogger _logger = new WalkDesignAdapterWrapperLogger();
    }
}