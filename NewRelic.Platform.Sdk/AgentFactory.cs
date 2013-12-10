using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using NewRelic.Platform.Sdk.Utils;
using NLog;

namespace NewRelic.Platform.Sdk
{
    /// <summary>
    /// An abstract utility class provided to easily create Agents from a configuration file
    /// </summary>
    public abstract class AgentFactory
    {
	    private readonly string ConfigurationFileName;

        private static Logger s_log = LogManager.GetLogger("Runner");

	    public AgentFactory(string configFileName) 
        {
            if (string.IsNullOrEmpty(configFileName))
            {
                throw new ArgumentNullException("configFileName", "You must pass in a non-null path for the configuration file");
            }

		    this.ConfigurationFileName = configFileName;
	    }

        internal List<Agent> CreateAgents()
        {
            var agentConfigurations = ReadJsonFile();
            List<Agent> agents = new List<Agent>();

            foreach (var properties in agentConfigurations)
            {
                agents.Add(CreateAgentWithConfiguration((IDictionary<string, object>)properties));
            }

            return agents;
        }

        internal List<object> ReadJsonFile()
        {
            string filePath = null;

            // First check if they've explicitly configured a target dir, or else use the default folder
            if (File.Exists(Path.Combine(ConfigurationHelper.GetConfiguration(Constants.ConfigKeyConfigDir, Constants.DefaultConfigDir), this.ConfigurationFileName)))
            {
                filePath = Path.Combine(ConfigurationHelper.GetConfiguration(Constants.ConfigKeyConfigDir, Constants.DefaultConfigDir), this.ConfigurationFileName);
            }
            // Fall back to the current directory of the executable (Global configuration for all users)
            else if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), this.ConfigurationFileName)))
            {
                filePath = Path.Combine(Directory.GetCurrentDirectory(), this.ConfigurationFileName);
            }
            else
            {
                throw new FileNotFoundException("Unable to locate agent configuration file", this.ConfigurationFileName);
            }

            s_log.Info("Using configuration file located at: {0}", filePath);

            using (var reader = new StreamReader(filePath))
            {
                using (var jsonReader = new JsonTextReader(reader))
                {
                    var agentProperties = JsonHelper.Deserialize(reader.ReadToEnd());
                    return (List<object>)agentProperties;
                }
            }
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
