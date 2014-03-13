using System;

namespace NewRelic.Platform.Sdk.Configuration
{
    public interface INewRelicConfig
    {
        string Endpoint { get; set; }

        string LicenseKey { get; set; }

        LogLevel LogLevel { get; set; }

        int? NewRelicMaxIterations { get; set; }

        string ProxyHost { get; set; }

        string ProxyPassword { get; set; }

        int? ProxyPort { get; set; }

        string ProxyUserName { get; set; }
    }
}
