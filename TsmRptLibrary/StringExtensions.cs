using System;
using System.Collections.Generic;
using System.Linq;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal static class StringExtensions
    {
        public static bool IsEquivalentTo(this string inputString, string compareTo)
        {
            return string.Equals(inputString, compareTo, StringComparison.OrdinalIgnoreCase);
        }

        public static bool Contains(this IEnumerable<string> myArray, string findString)
        {
            return myArray.Any(s => string.Equals(findString, s, StringComparison.OrdinalIgnoreCase));
        }

        public static bool EqualsCaseInsensitive(this string leftSide, string rightSide)
            => string.Equals(rightSide, leftSide, StringComparison.OrdinalIgnoreCase);

        public static int GetHours(this string inputString, int defaultValue)
        {
            var inputStringSubstring = inputString;
            int outputString;

            if ((inputString.Length > 2) && inputString.Contains(":"))
            {
                inputStringSubstring = inputString.Substring(0, inputString.IndexOf(':'));
            }

            return int.TryParse(inputStringSubstring, out outputString) ? outputString : defaultValue;
        }

        public static bool IfBoolThenParseElseDefault(this string inputString, bool defaultValue)
        {
            bool myBoolValue;
            var isBool = bool.TryParse(inputString, out myBoolValue);
            return isBool ? myBoolValue : defaultValue;
        }

        public static int IfIntThenParseElseDefault(this string inputString, int defaultValue)
        {
            int parsedInt;
            var isInt = int.TryParse(inputString.Trim(), out parsedInt);
            return isInt ? parsedInt : defaultValue;
        }

        public static bool IsInt(this string inputString)
        {
            int validInt;
            return int.TryParse(inputString.Trim(), out validInt);
        }
    }
}