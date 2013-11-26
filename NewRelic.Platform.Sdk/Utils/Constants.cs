using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NewRelic.Platform.Sdk
{
    internal static class Constants
    {
        public static readonly string ConfigKeyServiceUri = "NewRelicServiceUri";
        public static readonly string ConfigKeyPollInterval = "NewRelicPollInterval";
        public static readonly string ConfigKeyLicenseKey = "NewRelicLicenseKey";

        public static readonly string DefaultServiceUri = "https://collector.newrelic.com/platform/v1/metrics";
        public static readonly string DefaultPollInterval = "60";

        public static readonly int DefaultDuration = 60;

        public static readonly string DisableNewRelic = "DISABLE_NEWRELIC";
    }
}
