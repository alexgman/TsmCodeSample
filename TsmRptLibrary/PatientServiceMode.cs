using Profusion.Services.coffee.OsdRptLibrary.EppWebService;
using System;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class personTableStand : IpersonTableStand
    {
        private readonly personTableStandLogger _logger = new personTableStandLogger();

        public enum TableStand
        {
            Common = 0,
            Mct = 1,
            Cem = 2,
            Unknown = 3
        }

        public virtual TableStand GetTableStand(IDeviceStatusReader deviceStatusReader, Guid ptGuid)
        {
            var mode = deviceStatusReader.GetpersonDeviceStatus(ptGuid);

            if (mode == null)
            {
                this._logger.UnknownTableStand(ptGuid);
                return TableStand.Unknown;
            }

            this._logger.GotTableStand(mode);

            if (mode.IsCommonServices)
            {
                return TableStand.Common;
            }

            if (mode.ServiceType.IsEquivalentTo("CEM"))
            {
                return TableStand.Cem;
            }

            return mode.ServiceType.IsEquivalentTo("MCT") ? TableStand.Mct : TableStand.Unknown;
        }
    }
}