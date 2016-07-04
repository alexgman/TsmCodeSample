using log4net;
using Profusion.Services.coffee.Model;
using System;
using System.Messaging;
using System.Reflection;
using System.Threading;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class MessageReader
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected ILog GetLogger()
        {
            return LogManager.GetLogger(this.GetType().Assembly, this.GetType().Name + '.' + Thread.CurrentThread.ManagedThreadId);
        }

        public StartupAutomation ParseMessage(Message message)
        {
            return message.DeserializeToJson<StartupAutomation>();
        }

        public void Receive(MessageQueueWrapper messageQueue)
        {
            messageQueue.Receive(TimeSpan.FromSeconds(10));
        }

        public Message GetNextMessage(MessageQueueWrapper messageQueue, string queueAddress)
        {
            if (messageQueue.IsQueueEmpty())
            {
                return null;
            }

            return messageQueue.PeekZero();
        }
    }
}