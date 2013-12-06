using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NewRelic.Platform.Sdk;

namespace NewRelic.Platform.Sdk.FunctionalTests
{
    class TestAgentFactory : AgentFactory
    {
        public TestAgentFactory()
            : base("test-config.json")
        {
        }

        public TestAgentFactory(string configFileName)
            : base(configFileName)
        {
        }

        // This will return the deserialized properties from the specified configuration file
        // It will be invoked once per JSON object in the configuration file
        public override Agent CreateAgentWithConfiguration(IDictionary<string, object> properties)
        {
            string name = (string)properties["name"];
            double value = (double)properties["value"];

            return new TestAgent(name, (float)value);
        }
    }
}
