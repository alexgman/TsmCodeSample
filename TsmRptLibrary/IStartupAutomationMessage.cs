using System.Runtime.Serialization.Json;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal interface IStartupAutomationMessage
    {
        StartupAutomationWrapper DeserializedMessage(DataContractJsonSerializer typeOfSerializer);
    }
}