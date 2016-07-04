namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class coffeeRepositoryOperator
    {
        private readonly IcoffeeRepository _coffeeRepository;

        public coffeeRepositoryOperator(IcoffeeRepository coffeeRepository)
        {
            this._coffeeRepository = coffeeRepository;
        }

        public virtual long GetDeviceBySerialNumber(string serialNumber) => this._coffeeRepository.GetDeviceBySerialNumber(serialNumber).Id;

        public virtual void SaveChanges() => this._coffeeRepository.SaveChanges();
    }
}