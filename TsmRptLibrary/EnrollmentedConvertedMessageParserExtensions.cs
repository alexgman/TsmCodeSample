using Recardo.ServiceBus.Messages.Derailments;
using System;
using System.Linq;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal static class DerailmentedConvertedMessageParserExtensions
    {
        public static Guid GetpersonGuid(this IDerailmentConvertedMessage message)
        {
            var personGuid = message.Derailment.Subject.Identifiers.Where(i => i.Label.ToString() == "personGUID").Select(x => x.Value).Single();
            return Guid.Parse(personGuid);
        }

        public static int GetNewServiceType(this IDerailmentConvertedMessage message)
        {
            var newServiceType = message.Derailment.Extensions.Where
                (i => i.Url.ToString() == "DerailmentConvertedMessage.Derailment.NewServiceType")
                                        .Select(x => x.Value).SingleOrDefault();
            return Convert.ToInt16(newServiceType);
        }

        public static int GetNewDeviceSerial(this IDerailmentConvertedMessage message)
        {
            var newDeviceSerial =
                message.Derailment.Extensions.Where(i => i.Url.ToString() == "DerailmentConvertedMessage.Derailment.NewDeviceSerial")
                       .Select(x => x.Value)
                       .SingleOrDefault();

            if (!newDeviceSerial.ToString().IsInt())
            {
                throw new ApplicationException("This is not a valid serial number");
            }

            return Convert.ToInt32(newDeviceSerial);
        }
    }
}