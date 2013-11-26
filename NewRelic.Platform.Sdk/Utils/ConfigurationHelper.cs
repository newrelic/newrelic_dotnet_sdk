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
            return System.Configuration.ConfigurationManager.AppSettings[key] ?? GetDefaultConfigurationValue(key);
        }

        private static string GetDefaultConfigurationValue(string key)
        {
            switch (key)
            {
                case "NewRelicServiceUri":
                    return Constants.DefaultServiceUri;
                case "NewRelicPollInterval":
                    return Constants.DefaultPollInterval;
                default:
                    throw new ConfigurationErrorsException(string.Format("Requested configuration value '{0}' could not be found", key));
            }
        }
    }
}
