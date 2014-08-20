using System.Collections.Generic;

namespace NewRelic.Platform.Sdk.FunctionalTests
{
    class TestAgentFactory : AgentFactory
    {
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
