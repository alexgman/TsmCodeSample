using System;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    [Serializable]
    internal class MissingAppSettingException : Exception
    {
        public MissingAppSettingException(string key)
            : base($@"An expected appSettings value with key ""{key}"" is missing.")
        {
        }

        internal MissingAppSettingException(string key, Type expectedType)
            : base($@"An expected appSettings value with key ""{key}"" and type {expectedType.FullName} is missing.")
        {
        }
    }
}