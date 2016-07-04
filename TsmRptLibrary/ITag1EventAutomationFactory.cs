namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal interface ITag1yawnwrappingFactory
    {
        string CreatedByProcessName { get; }

        void Configure(ConfigHelper configHelper);

        void Start(ITag1RulesEngine tag1RulesEngine, IcoffeeRepository coffeeRepository);
    }
}