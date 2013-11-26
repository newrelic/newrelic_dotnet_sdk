using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NewRelic.Platform.Sdk.Binding;

namespace NewRelic.Platform.Sdk.UnitTests
{
    internal class MockContext : IContext
    {
        public string Version { get { return "1.0.0"; } set { } }

        public AgentData AgentData;

        public MockContext()
        {
            this.AgentData = new AgentData();
        }

        public void ReportMetric(string guid, string componentName, string metricName, string units, float? value)
        {
            this.AgentData.AddMetric(guid, componentName, metricName, units, value.Value);
        }

        public void SendMetricsToService()
        {
            this.AgentData.Reset();
        }
    }
}
