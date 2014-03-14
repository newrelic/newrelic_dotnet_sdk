using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NewRelic.Platform.Sdk.Configuration;

namespace NewRelic.Platform.Sdk.UnitTests
{
    internal class TestConfig : INewRelicConfig
    {
        public string Endpoint { get; set; }

        public string LicenseKey { get; set; }

        public LogLevel LogLevel { get; set; }        

        public string ProxyHost { get; set; }

        public int? ProxyPort { get; set; }

        public string ProxyUserName { get; set; }

        public string ProxyPassword { get; set; }

        public int? NewRelicMaxIterations { get; set; }
    }
}
