using System;
using Recardo.EnterpriseServices.globe.Contracts;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal interface IglobeSettingsReader
    {
        SettingValueBindingCollection GetSettingsForperson(Guid ptGuid);
    }
}