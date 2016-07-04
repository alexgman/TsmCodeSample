using Profusion.Services.coffee.Model;
using System.Messaging;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal interface IMessageReader
    {
        void Configure(ConfigHelper configHelper);

        StartupAutomation ParseMessage(Message message);

        Message GetNextMessage(MessageQueue queue, string queueAddress);
    }
}