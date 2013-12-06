using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NewRelic.Platform.Sdk.FunctionalTests
{
    internal class TestAgent : Agent
    {
        public override string Guid { get { return "com.newrelic.sdkfunctest"; } }
        public override string Version { get { return "1.0.0"; } }

        private string _name;
        private float _value;

        public TestAgent(string name, float value)
        {
            _name = name;
            _value = value;
        }

        public override string GetAgentName()
        {
            return _name;
        }

        public override void PollCycle()
        {
            ReportMetric("Category/TestMetric", "unit", _value);
        }
    }
}
