using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal static class AppSettingsExtensions
    {
        public static string Required(this NameValueCollection appSettings, string key)
        {
            var settingsValue = appSettings[key];
            if (string.IsNullOrEmpty(settingsValue))
                throw new MissingAppSettingException(key);
            return settingsValue;
        }

        public static string ValueOrDefault(this NameValueCollection appSettings, string key, string defaultValue)
        {
            return appSettings[key] ?? defaultValue;
        }

        public static T Get<T>(string key)
        {
            var appSetting = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(appSetting)) throw new ConfigurationErrorsException(key);

            var converter = TypeDescriptor.GetConverter(typeof(T));
            return (T)(converter.ConvertFromInvariantString(appSetting));
        }
    }
}