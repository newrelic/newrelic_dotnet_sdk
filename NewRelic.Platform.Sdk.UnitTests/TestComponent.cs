using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NewRelic.Platform.Sdk.UnitTests
{
    internal class TestComponent : Component
    {
        public override string Guid { get { return "com.newrelic.test"; } }
        public override string Version { get { return "1.0.0"; } }

        private string _name;

        public TestComponent(string name)
        {
            _name = name;
        }

        public override string GetComponentName()
        {
            return _name;
        }

        public override void PollCycle()
        {
            ReportMetric("Category/TestMetric", "unit", 2);
        }
    }
}
