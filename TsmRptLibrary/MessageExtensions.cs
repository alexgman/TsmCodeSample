using System.Messaging;
using System.Runtime.Serialization.Json;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal static class MessageExtensions
    {
        public static T DeserializeToJson<T>(this Message message)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            return (T)serializer.ReadObject(message.BodyStream);
        }
    }
}