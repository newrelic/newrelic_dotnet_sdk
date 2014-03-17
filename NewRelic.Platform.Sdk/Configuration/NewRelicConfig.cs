using System.Globalization;
using System.IO;
using Newtonsoft.Json;

namespace NewRelic.Platform.Sdk.Configuration
{
    internal class NewRelicConfig : INewRelicConfig
    {
        private static NewRelicConfig ConfigInstance;
        private const string ConfigPath = @".\config\newrelic.json";
        private const string DefaultEndpoint = "http://platform-api.newrelic.com/platform/v1/metrics";
        private const string DefaultLogFileName = "newrelic_plugin.log";
        private const string DefaultLogFilePath = @".\logs";
        private const int DefaultLogLimitInKiloBytes = 25600;

        private NewRelicConfig()
        {
            // set default values
            this.Endpoint = DefaultEndpoint;
            this.LogLevel = LogLevel.Info;
            this.LogFileName = DefaultLogFileName;
            this.LogFilePath = DefaultLogFilePath;
            this.LogLimitInKiloBytes = DefaultLogLimitInKiloBytes;
        }

        [JsonProperty(PropertyName = "license_key")]
        public string LicenseKey { get; set; }

        public string Endpoint { get; set; }

        [JsonProperty(PropertyName = "log_level")]
        public LogLevel LogLevel { get; set; }

        [JsonProperty(PropertyName = "log_filename")]
        public string LogFileName { get; set; }

        [JsonProperty(PropertyName = "log_file_path")]
        public string LogFilePath { get; set; }

        [JsonProperty(PropertyName = "log_limit_in_kbytes")]
        public long LogLimitInKiloBytes { get; set; }

        [JsonProperty(PropertyName = "proxy_host")]
        public string ProxyHost { get; set; }

        [JsonProperty(PropertyName = "proxy_port")]
        public int? ProxyPort { get; set; }

        [JsonProperty(PropertyName = "proxy_username")]
        public string ProxyUserName { get; set; }

        [JsonProperty(PropertyName = "proxy_password")]
        public string ProxyPassword { get; set; }

        // Exposed for testing
        public int? NewRelicMaxIterations { get; set; }

        public static NewRelicConfig Instance
        {
            get
            {
                if (ConfigInstance == null)
                {
                    if (!File.Exists(ConfigPath))
                    {
                        throw new FileNotFoundException(string.Format(
                            CultureInfo.InvariantCulture,
                            "New relic configuration needs to be located at {0}",
                            Path.GetFullPath(ConfigPath)));
                    }

                    ConfigInstance = JsonConvert.DeserializeObject<NewRelicConfig>(File.ReadAllText(ConfigPath));
                }

                return ConfigInstance;
            }
        }
    }
}
