using log4net;
using System;
using System.Messaging;
using System.Reflection;
using System.Threading;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class MessageQueueWrapper : IDisposable, IMessageQueue
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected ILog GetLogger()
        {
            return LogManager.GetLogger(this.GetType().Assembly, this.GetType().Name + '.' + Thread.CurrentThread.ManagedThreadId);
        }

        private readonly MessageQueue _messageQueue;

        public void Dispose()
        {
            this._messageQueue?.Dispose();
        }

        public MessageEnumerator GetMessageEnumerator2()
        {
            return this._messageQueue.GetMessageEnumerator2();
        }

        public MessageQueueWrapper(string path)
        {
            this._messageQueue = new MessageQueue(path);
        }

        public MessagePropertyFilter MessageReadPropertyFilter
        {
            get { return this._messageQueue.MessageReadPropertyFilter; }

            set
            {
                this._messageQueue.MessageReadPropertyFilter = value;
            }
        }

        public Message[] GetAllMessages()
        {
            return this._messageQueue.GetAllMessages();
        }

        public Message Peek()
        {
            return this._messageQueue.Peek();
        }

        public Message Peek(TimeSpan timeout)
        {
            return this._messageQueue.Peek(timeout);
        }

        public Message Receive()
        {
            return this._messageQueue.Receive();
        }

        public Message Receive(TimeSpan timeout)
        {
            try
            {
                return this._messageQueue.Receive(timeout);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Message Receive(MessageQueueTransaction transaction)
        {
            return this._messageQueue.Receive(transaction);
        }

        public Message Receive(MessageQueueTransactionType transactionType)
        {
            return this._messageQueue.Receive(transactionType);
        }

        public Message Receive(TimeSpan timeout, MessageQueueTransaction transaction)
        {
            return this._messageQueue.Receive(timeout, transaction);
        }

        public Message Receive(TimeSpan timeout, MessageQueueTransactionType transactionType)
        {
            return this._messageQueue.Receive(timeout, transactionType);
        }

        public Message Receive(TimeSpan timeout, Cursor cursor)
        {
            return this._messageQueue.Receive(timeout, cursor);
        }

        public Message Receive(TimeSpan timeout, Cursor cursor, MessageQueueTransaction transaction)
        {
            return this._messageQueue.Receive(timeout, cursor, transaction);
        }

        public Message Receive(TimeSpan timeout, Cursor cursor, MessageQueueTransactionType transactionType)
        {
            return this._messageQueue.Receive(timeout, cursor, transactionType);
        }

        public void Send(object obj)
        {
            this._messageQueue.Send(obj);
        }

        public void Send(object obj, string label)
        {
            this._messageQueue.Send(obj, label);
        }

        public void Send(object obj, MessageQueueTransactionType transactionType)
        {
            this._messageQueue.Send(obj, transactionType);
        }

        public void Send(object obj, MessageQueueTransaction transaction)
        {
            this._messageQueue.Send(obj, transaction);
        }

        public void Send(object obj, string label, MessageQueueTransactionType transactionType)
        {
            this._messageQueue.Send(obj, label, transactionType);
        }

        public void Send(object obj, string label, MessageQueueTransaction transaction)
        {
            this._messageQueue.Send(obj, label, transaction);
        }
    }
}