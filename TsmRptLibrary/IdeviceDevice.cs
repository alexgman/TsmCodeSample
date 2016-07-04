using Profusion.Services.coffee.Adapter;
using Profusion.Services.coffee.DataParser;
using Profusion.Services.coffee.OsdRptLibrary.EppWebService;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal interface IcoffeeDevice
    {
        bool DoNewSettingsNeedToBeQueued { get; }
        bool IsGettingPvcCounts { get; }
        bool IsInMctOrCommonMode { get; }
        bool IsPvcEnabled { get; }
        bool IscoffeeDevice { get; }
        DeviceStatus personDeviceStatus { get; }
        int personId { get; }
        string SerialNumber { get; }
        DeviceSettings coffeeDeviceSettings { get; }
        VeritéDeviceInteractions coffeeInteractions { get; }

        void InitializeDeviceSettings();

        void InitializepersonDeviceStatus();

        bool IsFacilityGettingPvcCounts(int facilityid);

        void QueueDeviceSettings();
    }
}