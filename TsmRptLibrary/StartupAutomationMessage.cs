using System.Messaging;
using System.Runtime.Serialization.Json;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class StartupAutomationMessage : IStartupAutomationMessage
    {
        private readonly Message _serializedMessage;

        public virtual StartupAutomationWrapper DeserializedMessage(DataContractJsonSerializer typeOfSerializer)
        {
            return (StartupAutomationWrapper)typeOfSerializer.ReadObject(this._serializedMessage.BodyStream);
        }

        public StartupAutomationMessage(Message serializedMessage)
        {
            this._serializedMessage = serializedMessage;
        }
    }
}