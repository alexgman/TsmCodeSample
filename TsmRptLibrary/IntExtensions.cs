namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal static class IntExtensions
    {
        public static bool IsBetween(this int inputInt, int lower, int upper, bool inclusive = true)
            => inclusive ? (lower <= inputInt) && (inputInt <= upper) : (lower < inputInt) && (inputInt < upper);
    }
}