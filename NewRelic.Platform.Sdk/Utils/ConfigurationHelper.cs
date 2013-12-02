using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Text;

namespace NewRelic.Platform.Sdk.Utils
{
    internal static class ConfigurationHelper
    {
        internal static string GetConfiguration(string key)
        {
            return GetConfiguration(key, null);
        }

        internal static string GetConfiguration(string key, string defaultValue)
        {
            var configuration = System.Configuration.ConfigurationManager.AppSettings[key] ?? defaultValue;

            if (configuration == null)
            {
                throw new ConfigurationErrorsException(string.Format("Requested configuration value '{0}' could not be found", key));
            }

            return configuration;
        }
    }
}
