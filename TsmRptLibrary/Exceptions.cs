using System;
using System.Runtime.Serialization;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    [Serializable]
    internal class UnableToDetermineServiceType : Exception
    {
        public UnableToDetermineServiceType(string message)
            : base(message)
        {
            throw new NullReferenceException(message);
        }

        public UnableToDetermineServiceType(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected UnableToDetermineServiceType(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    internal class MissingptGuidForCurrentSerialException : Exception
    {
        public MissingptGuidForCurrentSerialException(string message) : base(message)
        {
            throw new ArgumentException(message);
        }
    }

    [Serializable]
    internal class SerialNumberIsEmptyForCurrentpersonException : Exception
    {
        public SerialNumberIsEmptyForCurrentpersonException(string message) : base(message)
        {
            throw new ArgumentException(message);
        }
    }

    [Serializable]
    internal class EmptyConnectionStringException : Exception
    {
        public EmptyConnectionStringException(string message) : base(message)
        {
            throw new ArgumentException(message);
        }
    }
}