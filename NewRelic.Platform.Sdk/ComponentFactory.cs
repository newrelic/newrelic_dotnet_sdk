using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using NewRelic.Platform.Sdk.Utils;

namespace NewRelic.Platform.Sdk
{
    /// <summary>
    /// An abstract utility class provided to easily create Components from a configuration file
    /// </summary>
    public abstract class ComponentFactory
    {
	    private readonly string ConfigurationFileName;

	    public ComponentFactory(string configFileName) 
        {
            if (string.IsNullOrEmpty(configFileName))
            {
                throw new ArgumentNullException("configFileName", "You must pass in a non-null path for the configuration file");
            }

		    this.ConfigurationFileName = configFileName;
	    }

        internal List<Component> CreateComponents()
        {
            var componentConfigurations = ReadJsonFile();
            List<Component> components = new List<Component>();

            foreach (var properties in componentConfigurations)
            {
                components.Add(CreateComponentWithConfiguration((IDictionary<string, object>)properties));
            }

            return components;
        }

        internal List<object> ReadJsonFile()
        {
            if(!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), this.ConfigurationFileName)))
            {
                throw new FileNotFoundException("Unable to locate component configuration file", this.ConfigurationFileName);
            }

            using (var reader = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), this.ConfigurationFileName)))
            {
                using (var jsonReader = new JsonTextReader(reader))
                {
                    var componentProperties = JsonHelper.Deserialize(reader.ReadToEnd());
                    return (List<object>)componentProperties;
                }
            }
        }

        /// <summary>
        /// The ComponentFactory will read configuration data from specified JSON file and invoke this method for each
        /// object configuration containing an IDictionary of the deserialized properties
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public abstract Component CreateComponentWithConfiguration(IDictionary<string, object> properties);
    }
}
