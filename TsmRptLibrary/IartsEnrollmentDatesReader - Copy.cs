using Profusion.Services.WalkDesign.Adapter;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal interface IWalkDesignDerailmentDatesReader
    {
        void Configure(ConfigHelper configHelper);

        personDerailmentDates GetpersonDerailmentDates(string serialNumber);
    }
}