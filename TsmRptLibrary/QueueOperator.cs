using System;
using System.IO;
using System.Messaging;
using System.Runtime.Serialization.Json;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class QueueOperator
    {
        private readonly ConfigHelper _configHelper;
        private readonly DateTime _startTimeOfProcessing;

        public QueueOperator(DateTime startTimeOfProcessing, ConfigHelper configHelper)
        {
            if (configHelper == null)
            {
                throw new MissingAppSettingException("configHelper");
            }
            this._startTimeOfProcessing = startTimeOfProcessing;
            this._configHelper = configHelper;
        }

        public bool PopMessageOffQueue(IMessageQueue messageQueue) => messageQueue.Receive(TimeSpan.Zero) == null;

        private Message PeekAtMessage(IMessageQueue messageQueue) => messageQueue.Peek(TimeSpan.Zero);

        public IStartupAutomation GetNextMessage(IMessageQueue messageQueue, IStartupAutomationMessage startupAutomationMessage)
        {
            var currentMessage = new StartupAutomationMessage(this.PeekAtMessage(messageQueue));

            var deserializedMessage = currentMessage.DeserializedMessage(new DataContractJsonSerializer(typeof(StartupAutomationWrapper)));

            if (this.IsOurProcessOld(deserializedMessage.LastAttemptDate))
            {
                return null;
            }

            return deserializedMessage;
        }

        public bool IsQueueEmpty(IMessageQueue messageQueue) => messageQueue.Peek(TimeSpan.Zero) == null;

        private bool IsOurProcessOld(DateTime currentMessageDateTime) => currentMessageDateTime > this._startTimeOfProcessing;

        public void PutMessageBackOnQueue(IStartupAutomation messageToRequeue)
        {
            //Log.Debug("Requeuing Tag 1 MSMQ message...");

            var startupAutomationDataSerializer = new DataContractJsonSerializer(typeof(IStartupAutomation));
            var memoryStream = new MemoryStream();
            startupAutomationDataSerializer.WriteObject(memoryStream, messageToRequeue);

            memoryStream.Position = 0;

            var messageQueueTransaction = new MessageQueueTransaction();
            var message = new Message
            {
                Label = messageToRequeue.Tze.Filename,
                BodyStream = memoryStream
            };

            var messageQueue = new MessageQueue(this._configHelper.coffeeAutomationQueueLocation);

            messageQueueTransaction.Begin();
            messageQueue.Send(message, messageQueueTransaction);
            messageQueueTransaction.Commit();

            //Log.Info("Message added to MessageQueue: " + CurrentMessage.Tze.Filename + " | " + CurrentMessage.Tze.Tag);
        }
    }
}