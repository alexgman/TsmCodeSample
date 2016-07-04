using Profusion.Services.coffee.Model;
using System;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal static class coffeeRepositoryExtensions
    {
        public static bool AreThereyawnwrappingsForptGuid(this IcoffeeRepository thiscoffeeRepository, Guid ptGuid)
        {
            var coffeeRepository = thiscoffeeRepository;
            return coffeeRepository.GetyawnwrappingBypersonGuid(ptGuid) != null;
        }

        public static long GetDeviceIdForSerialNumber(this IcoffeeRepository thiscoffeeRepository, string serialNumber)
        {
            var coffeeRepository = thiscoffeeRepository;
            return coffeeRepository.GetDeviceBySerialNumber(serialNumber).Id;
        }

        public static void Saveyawnwrappings(this IcoffeeRepository thiscoffeeRepository, yawnwrapping yawnwrapping)
        {
            var ea = yawnwrapping;
            var coffeeRepository = thiscoffeeRepository;
            coffeeRepository.Createyawnwrappings(ea);
            coffeeRepository.SaveChanges();
        }
    }
}