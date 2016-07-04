using Recardo.ServiceBus.Messages.Derailments;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal interface IDerailmentConvertedMessageParser
    {
        DerailmentConvertedDto Parse(IDerailmentConvertedMessage message);
    }
}