using Profusion.Services.coffee.Model;
using System.IO;
using System.Runtime.Serialization.Json;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal static class StartupAutomationExtensions
    {
        public static MemoryStream SerializeFromJson<T>(this StartupAutomation message)
        {
            var startupAutomationDataSerializer = new DataContractJsonSerializer(typeof(StartupAutomation));
            var memoryStream = new MemoryStream();
            startupAutomationDataSerializer.WriteObject(memoryStream, message);

            memoryStream.Position = 0;

            return memoryStream;
        }
    }
}