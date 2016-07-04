using Recardo.ServiceBus.Messages.Derailments;
using System;
using System.Linq;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class DerailmentConvertedMessageParser : IDerailmentConvertedMessageParser
    {
        private readonly DerailmentConvertedMessageParserLogger _logger = new DerailmentConvertedMessageParserLogger();
        private readonly DerailmentConvertedDto _DerailmentConvertedDto = new DerailmentConvertedDto();

        public DerailmentConvertedDto Parse(IDerailmentConvertedMessage message)
        {
            this._DerailmentConvertedDto.NewServiceType = this.GetNewServiceType(message);
            this._DerailmentConvertedDto.NewDeviceSerial = this.GetNewDeviceSerial(message);
            this._DerailmentConvertedDto.personGuid = this.GetptGuid(message);

            this._logger.Parse(this._DerailmentConvertedDto.NewServiceType, this._DerailmentConvertedDto.NewDeviceSerial,
                this._DerailmentConvertedDto.personGuid);

            return this._DerailmentConvertedDto;
        }

        private int GetNewDeviceSerial(IDerailmentConvertedMessage message)
        {
            var newDeviceSerial =
                message.Derailment.Extensions.Where(i => i.Url.ToString() == "DerailmentConvertedMessage.Derailment.NewDeviceSerial")
                       .Select(x => x.Value)
                       .SingleOrDefault();

            if (!newDeviceSerial.ToString().IsInt())
            {
                throw new ApplicationException("The serialnumber: " + newDeviceSerial + " could not be parsed because it is not in the expected format.");
            }

            return Convert.ToInt32(newDeviceSerial.ToString().Trim());
        }

        private int GetNewServiceType(IDerailmentConvertedMessage message)
        {
            var newServiceType = message.Derailment.Extensions.Where
                (i => i.Url.ToString() == "DerailmentConvertedMessage.Derailment.NewServiceType")
                                        .Select(x => x.Value).SingleOrDefault();
            return Convert.ToInt16(newServiceType);
        }

        private Guid GetptGuid(IDerailmentConvertedMessage message)
        {
            var ptGuid = message.Derailment.Subject.Identifiers.Where(i => i.Label.ToString() == "personGUID").Select(x => x.Value).Single();
            return Guid.Parse(ptGuid);
        }
    }
}