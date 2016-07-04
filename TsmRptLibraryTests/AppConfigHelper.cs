using System;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace ToggleServiceModeTests
{
    public abstract class AppConfigHelper : IDisposable
    {
        public static AppConfigHelper Change(string path)
        {
            return new ChangeAppConfigHelper(path);
        }

        public abstract void Dispose();

        private class ChangeAppConfigHelper :AppConfigHelper
        {
            private readonly string oldConfig =
                AppDomain.CurrentDomain.GetData("APP_CONFIGFILE").ToString();

            private bool disposedValue;

            public ChangeAppConfigHelper(string path)
            {
                AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", path);
                ResetConfigMechanism();
            }

            public override void Dispose()
            {
                if (!this.disposedValue)
                {
                    AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", this.oldConfig);
                    ResetConfigMechanism();

                    this.disposedValue = true;
                }
                GC.SuppressFinalize(this);
            }

            private static void ResetConfigMechanism()
            {
                typeof(ConfigurationManager)
                    .GetField("s_initState", BindingFlags.NonPublic |
                                             BindingFlags.Static)
                    .SetValue(null, 0);

                typeof(ConfigurationManager)
                    .GetField("s_configSystem", BindingFlags.NonPublic |
                                                BindingFlags.Static)
                    .SetValue(null, null);

                typeof(ConfigurationManager)
                    .Assembly.GetTypes()
                    .Where(x => x.FullName ==
                                "System.Configuration.ClientConfigPaths")
                    .First()
                    .GetField("s_current", BindingFlags.NonPublic |
                                           BindingFlags.Static)
                    .SetValue(null, null);
            }
        }
    }
}