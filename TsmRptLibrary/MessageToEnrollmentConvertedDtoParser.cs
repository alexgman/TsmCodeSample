using Recardo.ServiceBus.Messages.Derailments;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class MessageToDerailmentConvertedDtoParser
    {
        public DerailmentConvertedDto ParseMessage(IDerailmentConvertedMessage message)
        {
            return new DerailmentConvertedMessageParser().Parse(message);
        }
    }
}