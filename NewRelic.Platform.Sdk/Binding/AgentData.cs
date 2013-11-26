using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NewRelic.Platform.Sdk;

namespace NewRelic.Platform.Sdk.Binding
{
    internal class AgentData
    {
        private string _host = "host";
        private int _pid = 0;
        private DateTime _aggregationStartTime;
        private IDictionary<string, ComponentData> _components;

        internal string Version { get; set; }

        internal AgentData()
        {
            _components = new Dictionary<string, ComponentData>();
            _aggregationStartTime = DateTime.Now.Subtract(TimeSpan.FromSeconds(Constants.DefaultDuration)); // Set the default to 60 seconds in the past
        }

        internal void AddMetric(string guid, string componentName, string metricName, string units, float value) 
        {
            if (!_components.ContainsKey(componentName))
            {
                ComponentData componentData = new ComponentData(componentName, guid);
                _components.Add(componentName, componentData);
            }

            _components[componentName].AddMetric(new MetricData(metricName, units, value));
        }

        internal bool HasComponents()
        {
            return _components.Count > 0;
        }

        internal bool IsPastAggregationLimit()
        {
            return Convert.ToInt32((DateTime.Now - _aggregationStartTime).TotalMinutes) > 10; ;
        }

        internal void Reset()
        {
            _components = new Dictionary<string, ComponentData>();
            _aggregationStartTime = DateTime.Now;
        }

        internal IDictionary<string, object> Serialize()
        {
            IDictionary<string, object> output = new Dictionary<string, object>();
            output.Add("agent", SerializeAgent());
            output.Add("components", SerializeComponents());
            return output;
        }

        private IDictionary<string, object> SerializeAgent()
        {
            IDictionary<string, object> output = new Dictionary<string, object>();
            output.Add("host", _host);
            output.Add("version", this.Version);
            output.Add("pid", _pid);
            return output;
        }

        private List<object> SerializeComponents()
        {
            List<object> output = new List<object>();

            foreach (var componentData in _components.Values)
            {
                output.Add(componentData.Serialize(_aggregationStartTime));
            }

            return output;
        }

        #region Test Helpers
        /// <summary>
        /// DO NOT USE: Exposed for testing
        /// </summary>
        internal void SetAggregationLimit(DateTime aggregationStart)
        {
            _aggregationStartTime = aggregationStart;
        }
        #endregion
    }
}
