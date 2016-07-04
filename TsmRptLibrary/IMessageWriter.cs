using Profusion.Services.coffee.Model;
using System.Messaging;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal interface IMessageWriter
    {
        void Configure(ConfigHelper configHelper);

        void ReturnMessageToQueue(MessageQueue messageQueue, StartupAutomation message);
    }
}