using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NewRelic.Platform.Sdk.Binding
{
    public class RequestData
    {
        private DateTime _aggregationStartTime;

        private AgentData _agent;
        private IDictionary<string, ComponentData> _components;

        public string Version
        {
            get { return _agent.Version; }
            set { _agent.Version = value; }
        }

        public RequestData()
        {
            _agent = new AgentData();
            _components = new Dictionary<string, ComponentData>();
            _aggregationStartTime = DateTime.Now.Subtract(TimeSpan.FromSeconds(Constants.DefaultDuration)); // Set with the default duration
        }

        public void AddMetric(string guid, string componentName, string metricName, string units, float value)
        {
            string key = string.Format("{0}:{1}", componentName, guid);

            if (!_components.ContainsKey(key))
            {
                ComponentData componentData = new ComponentData(componentName, guid);
                _components.Add(key, componentData);
            }

            _components[key].AddMetric(new MetricData(metricName, units, value));
        }

        public void AddMetric(string guid, string componentName, string metricName, string units, float value, int count, float min, float max, float sumOfSquares)
        {
            string key = string.Format("{0}:{1}", componentName, guid);

            if (!_components.ContainsKey(key))
            {
                ComponentData componentData = new ComponentData(componentName, guid);
                _components.Add(key, componentData);
            }

            _components[key].AddMetric(new MetricData(metricName, units, count, value, min, max, sumOfSquares));
        }

        public bool HasComponents()
        {
            return _components.Count > 0;
        }

        public bool IsPastAggregationLimit()
        {
            return Convert.ToInt32((DateTime.Now - _aggregationStartTime).TotalMinutes) > Constants.DefaultAggregationLimit;
        }

        public void Reset()
        {
            _components = new Dictionary<string, ComponentData>();
            _aggregationStartTime = DateTime.Now;
        }

        public IDictionary<string, object> Serialize()
        {
            IDictionary<string, object> output = new Dictionary<string, object>();
            output.Add("agent", _agent.Serialize());
            output.Add("components", SerializeComponents());
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
