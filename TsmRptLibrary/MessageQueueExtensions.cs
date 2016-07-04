using System;
using System.IO;
using System.Messaging;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal static class MessageQueueExtensions
    {
        public static bool IsAbleToPop(this IMessageQueue thisMessageQueue, TimeSpan? timeSpan = null)
            => thisMessageQueue.Receive(timeSpan ?? TimeSpan.Zero) == null;

        public static bool IsQueueEmpty(this IMessageQueue thisMessageQueue)
        {
            var queueEnum = thisMessageQueue.GetMessageEnumerator2();
            return !queueEnum.MoveNext();
        }

        public static Message PeekZero(this IMessageQueue thisMessageQueue, TimeSpan? timeSpan = null)
            => thisMessageQueue.Peek(timeSpan ?? TimeSpan.Zero);

        public static void ReQueue(this IMessageQueue thisMessageQueue, MemoryStream bodyStream, string label)
        {
            var messageQueueTransaction = new MessageQueueTransaction();
            var message = new Message
            {
                Label = label,
                BodyStream = bodyStream
            };

            messageQueueTransaction.Begin();
            thisMessageQueue.Send(message, messageQueueTransaction);
            messageQueueTransaction.Commit();
        }
    }
}