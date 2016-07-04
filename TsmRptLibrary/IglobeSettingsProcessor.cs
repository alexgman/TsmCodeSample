using Recardo.EnterpriseServices.globe.Contracts;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal interface IglobeSettingsProcessor
    {
        globeSettingsDto GetSettings(SettingValueBindingCollection items);
    }
}