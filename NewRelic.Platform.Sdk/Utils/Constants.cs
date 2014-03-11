namespace NewRelic.Platform.Sdk
{
    internal static class Constants
    {
        public static readonly string DefaultServiceUri = "https://collector.newrelic.com/platform/v1/metrics";

        public static readonly int DefaultDuration = 60;
        public static readonly int DefaultAggregationLimit = 10;

        public static readonly string DisableNewRelic = "DISABLE_NEWRELIC";
    }
}
