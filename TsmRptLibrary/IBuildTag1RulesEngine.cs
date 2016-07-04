using System;

namespace TsmRptLibrary
{
    internal interface IBuildTag1RulesEngine
    {
        Tag1RulesEngine BuildRulesEngine(Guid patientGuid, string serialNumber, IdeviceRepository deviceRepository);
    }
}