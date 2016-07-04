using System;
using System.ComponentModel;
using System.IO;

namespace TsmRptLibrary
{
    internal class Message_ : Component, IMessage
    {
        private static readonly TimeSpan InfiniteTimeout;

        public AcknowledgeTypes AcknowledgeType
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public Acknowledgment Acknowledgment
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public MessageQueue AdministrationQueue
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int AppSpecific
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public DateTime ArrivedTime
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool AttachSenderId
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool Authenticated
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string AuthenticationProviderName
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public CryptographicProviderType AuthenticationProviderType
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public object Body
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public Stream BodyStream
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int BodyType
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public Guid ConnectorType
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public string CorrelationId
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public MessageQueue DestinationQueue
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public byte[] DestinationSymmetricKey
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public byte[] DigitalSignature
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public EncryptionAlgorithm EncryptionAlgorithm
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public byte[] Extension
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public IMessageFormatter Formatter
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public HashAlgorithm HashAlgorithm
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public string Id
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsFirstInTransaction
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsLastInTransaction
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Label
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public long LookupId
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public MessageType MessageType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public MessagePriority Priority
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool Recoverable
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public MessageQueue ResponseQueue
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public SecurityContext SecurityContext
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public byte[] SenderCertificate
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public byte[] SenderId
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public long SenderVersion
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public DateTime SentTime
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string SourceMachine
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public TimeSpan TimeToBeReceived
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public TimeSpan TimeToReachQueue
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public string TransactionId
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public MessageQueue TransactionStatusQueue
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool UseAuthentication
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool UseDeadLetterQueue
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool UseEncryption
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool UseJournalQueue
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool UseTracing
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
    }
}