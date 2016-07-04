using System;
using System.IO;

namespace TsmRptLibrary
{
    internal interface IMessage
    {
        AcknowledgeTypes AcknowledgeType { get; set; }
        Acknowledgment Acknowledgment { get; }
        MessageQueue AdministrationQueue { get; set; }
        int AppSpecific { get; set; }
        DateTime ArrivedTime { get; }
        bool AttachSenderId { get; set; }
        bool Authenticated { get; }
        string AuthenticationProviderName { get; set; }
        CryptographicProviderType AuthenticationProviderType { get; set; }
        object Body { get; set; }
        Stream BodyStream { get; set; }
        int BodyType { get; set; }
        Guid ConnectorType { get; set; }
        string CorrelationId { get; set; }
        MessageQueue DestinationQueue { get; }
        byte[] DestinationSymmetricKey { get; set; }
        byte[] DigitalSignature { get; set; }
        EncryptionAlgorithm EncryptionAlgorithm { get; set; }
        byte[] Extension { get; set; }
        IMessageFormatter Formatter { get; set; }
        HashAlgorithm HashAlgorithm { get; set; }
        string Id { get; }
        bool IsFirstInTransaction { get; }
        bool IsLastInTransaction { get; }
        string Label { get; set; }
        long LookupId { get; }
        MessageType MessageType { get; }
        MessagePriority Priority { get; set; }
        bool Recoverable { get; set; }
        MessageQueue ResponseQueue { get; set; }
        SecurityContext SecurityContext { get; set; }
        byte[] SenderCertificate { get; set; }
        byte[] SenderId { get; }
        long SenderVersion { get; }
        DateTime SentTime { get; }
        string SourceMachine { get; }
        TimeSpan TimeToBeReceived { get; set; }
        TimeSpan TimeToReachQueue { get; set; }
        string TransactionId { get; }
        MessageQueue TransactionStatusQueue { get; set; }
        bool UseAuthentication { get; set; }
        bool UseDeadLetterQueue { get; set; }
        bool UseEncryption { get; set; }
        bool UseJournalQueue { get; set; }
        bool UseTracing { get; set; }
    }
}