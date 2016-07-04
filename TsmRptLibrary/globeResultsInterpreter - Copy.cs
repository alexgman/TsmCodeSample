namespace TsmRptLibrary
{
    internal class AtlasResultsInterpreter
    {
        public static int Daycounter;

        public bool AreTimedEventsEnabled(bool enabledTimedEvents, int dayCounter, int timedDailyFrequency)
        {
            return enabledTimedEvents && (dayCounter % timedDailyFrequency == 0);
        }
    }
}