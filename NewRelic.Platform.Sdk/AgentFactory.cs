using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using NewRelic.Platform.Sdk.Utils;
using Newtonsoft.Json;

namespace NewRelic.Platform.Sdk
{
    /// <summary>
    /// An abstract utility class provided to easily create Agents from a configuration file
    /// </summary>
    public abstract class AgentFactory
    {
        private const string ConfigurationFilePath = @".\config\plugin.json";

        private static Logger s_log = Logger.GetLogger("AgentFactory");

        internal List<Agent> CreateAgents()
        {
            List<object> agentConfigurations = ReadJsonFile();
            List<Agent> agents = new List<Agent>();

            foreach (object properties in agentConfigurations)
            {
                agents.Add(CreateAgentWithConfiguration((IDictionary<string, object>)properties));
            }

            return agents;
        }

        internal List<object> ReadJsonFile()
        {
            if (!File.Exists(ConfigurationFilePath))
            {
                throw new FileNotFoundException(string.Format(
                    CultureInfo.InvariantCulture,
                    "Unable to locate agent configuration file at {0}",
                    Path.GetFullPath(ConfigurationFilePath)));
            }

            object agentProperties = JsonHelper.Deserialize(File.ReadAllText(ConfigurationFilePath));
            return (List<object>)agentProperties;
        }

        /// <summary>
        /// The AgentFactory will read configuration data from specified JSON file and invoke this method for each
        /// object configuration containing an IDictionary of the deserialized properties
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public abstract Agent CreateAgentWithConfiguration(IDictionary<string, object> properties);
    }
}
