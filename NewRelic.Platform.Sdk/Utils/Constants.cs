using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NewRelic.Platform.Sdk
{
    internal static class Constants
    {
        public static readonly string ConfigKeyServiceUri = "NewRelicServiceUrl";
        public static readonly string ConfigKeyPollInterval = "NewRelicPollInterval";
        public static readonly string ConfigKeyLicenseKey = "NewRelicLicenseKey";
        public static readonly string ConfigKeyConfigDir = "NewRelicConfigDir";

        public static readonly string DefaultServiceUri = "https://collector.newrelic.com/platform/v1/metrics";
        public static readonly string DefaultPollInterval = "60";

        public static readonly string DefaultConfigDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"New Relic\Plugins\Config");

        public static readonly int DefaultDuration = 60;
        public static readonly int DefaultAggregationLimit = 10;

        public static readonly string DisableNewRelic = "DISABLE_NEWRELIC";

        // Exposed for testing
        public static readonly string ConfigKeyMaxIterations = "NewRelicMaxIterations";
    }
}
