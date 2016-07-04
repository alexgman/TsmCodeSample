using System;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal static class Preconditions
    {
        private const int MinimumValidFacilityId = 1;
        private const int MinimumValidpersonId = 2;

        public static void CheckNotNull<T>(T value, string paramName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        public static void IsValidFacilityId(int facilityid, string paramName)
        {
            if (facilityid < MinimumValidFacilityId)
            {
                throw new ArgumentOutOfRangeException(paramName);
            }
        }
    }
}