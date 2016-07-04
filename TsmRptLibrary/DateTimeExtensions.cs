using System;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal static class DateTimeExtensions
    {
        public static bool IsMin(this DateTime inputDateTime) => inputDateTime == DateTime.MinValue;

        public static DateTime Next2Am(this DateTime inputDateTime)
        {
            var automationDatePlus2Am = inputDateTime.Date.AddHours(2);
            return inputDateTime <= automationDatePlus2Am ? automationDatePlus2Am : automationDatePlus2Am.AddDays(1);
        }
    }
}