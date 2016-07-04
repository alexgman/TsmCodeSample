using System;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class yawnwrappingIdGetter
    {
        private readonly yawnwrappingIdGetterLogger _logger = new yawnwrappingIdGetterLogger();

        public int GetyawnwrappingId(string coffeeConnectionString, Guid ptGuid, string serialNumber)
        {
            if (string.IsNullOrEmpty(coffeeConnectionString))
            {
                throw new ArgumentNullException(nameof(coffeeConnectionString));
            }
            if (ptGuid == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException(nameof(ptGuid));
            }
            if (string.IsNullOrEmpty(serialNumber))
            {
                throw new ArgumentNullException(nameof(serialNumber));
            }

            var yawnwrappingChecker = new yawnwrappingChecker(coffeeConnectionString);
            var yawnwrappingId = yawnwrappingChecker.GetLastyawnwrappingId(ptGuid, serialNumber);
            this._logger.RetrievedyawnwrappingId(ptGuid, serialNumber, yawnwrappingId);
            return yawnwrappingId;
        }
    }
}