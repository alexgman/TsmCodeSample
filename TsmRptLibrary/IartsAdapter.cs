using Profusion.Services.WalkDesign.Adapter;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal interface IWalkDesignAdapter
    {
        personDerailmentDates GetcoffeeWalkDesignMostRecentDerailmentFromDeviceSerialNoExplant(string serialNumber);
    }
}