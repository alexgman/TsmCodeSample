using System;
using System.Messaging;

//using Message = System.TableStandl.Channels.Message;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal interface IMessageQueue
    {
        MessagePropertyFilter MessageReadPropertyFilter { get; set; }

        void Dispose();

        Message[] GetAllMessages();

        MessageEnumerator GetMessageEnumerator2();

        Message Peek();

        Message Peek(TimeSpan timeout);

        Message Receive();

        Message Receive(TimeSpan timeout);

        Message Receive(MessageQueueTransaction transaction);

        Message Receive(MessageQueueTransactionType transactionType);

        Message Receive(TimeSpan timeout, MessageQueueTransaction transaction);

        Message Receive(TimeSpan timeout, MessageQueueTransactionType transactionType);

        Message Receive(TimeSpan timeout, Cursor cursor);

        Message Receive(TimeSpan timeout, Cursor cursor, MessageQueueTransaction transaction);

        Message Receive(TimeSpan timeout, Cursor cursor, MessageQueueTransactionType transactionType);

        void Send(object obj);

        void Send(object obj, string label);

        void Send(object obj, MessageQueueTransactionType transactionType);

        void Send(object obj, MessageQueueTransaction transaction);

        void Send(object obj, string label, MessageQueueTransactionType transactionType);

        void Send(object obj, string label, MessageQueueTransaction transaction);
    }
}