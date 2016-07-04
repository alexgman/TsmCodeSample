namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal static class TimedEventEntryExtensions
    {
        public static int HourIncrementor(this int timedtripsPerDay)
        {
            return timedtripsPerDay == 1
                ? 0
                : timedtripsPerDay == 3
                    ? 12
                    : timedtripsPerDay == 5 ? 6 : timedtripsPerDay == 11 ? 4 : timedtripsPerDay == 24 ? 1 : 2;
        }
    }
}