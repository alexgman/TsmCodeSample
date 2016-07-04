namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal interface ITag1QueueProcessor
    {
        void Configure(ConfigHelper configHelper);

        void ProcessMessages(IMessageQueue queue, string queueLocation);
    }
}