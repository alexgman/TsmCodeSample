using log4net;
using Profusion.Services.coffee.Model;
using System;
using System.Reflection;
using System.Threading;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class MessageWriter
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected ILog GetLogger()
        {
            return LogManager.GetLogger(this.GetType().Assembly, this.GetType().Name + '.' + Thread.CurrentThread.ManagedThreadId);
        }

        private MessageQueueWrapper _queue;

        public MessageWriter(MessageQueueWrapper queue)
        {
            this._queue = queue;
        }

        public void ReturnMessageToQueue(MessageQueueWrapper messageQueue, StartupAutomation message)
        {
            var startupMessage = this.IncrementRetryCounter(message);

            var memoryStream = startupMessage.SerializeFromJson<StartupAutomation>();
            messageQueue.ReQueue(memoryStream, message.Tze.Filename);
        }

        private StartupAutomation IncrementRetryCounter(StartupAutomation startupAutomation)
        {
            var updatedMessage = new StartupAutomation();

            updatedMessage.Tze = startupAutomation.Tze;
            updatedMessage.OriginalQueueDate = startupAutomation.OriginalQueueDate;
            updatedMessage.LastAttemptDate = DateTime.Now;
            updatedMessage.RetryCounter += 1;

            return updatedMessage;
        }
    }
}