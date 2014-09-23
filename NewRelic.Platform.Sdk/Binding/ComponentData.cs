using System;
using System.Collections.Generic;

namespace NewRelic.Platform.Sdk.Binding
{
    internal class ComponentData
    {
        #region Properties
        private string _name;
        private string _guid;

        internal string Name { get { return _name; } }
        internal string Guid { get { return _guid; } }
        #endregion

        private IDictionary<string, MetricData> _metrics;

        internal ComponentData(string name, string guid)
        {
            _name = name;
            _guid = guid;

            _metrics = new Dictionary<string, MetricData>();
        }

        internal void AddMetric(MetricData metric)
        {
            if (!_metrics.ContainsKey(metric.FullName))
            {
                _metrics.Add(metric.FullName, metric);
            }
            else
            {
                _metrics[metric.FullName].AggregateWith(metric);
            }
        }

        internal IDictionary<string, object> Serialize(DateTime lastSuccessfulReport)
        {
            IDictionary<string, object> output = new Dictionary<string, object>();

            output.Add("name", this.Name);
            output.Add("guid", this.Guid);
            output.Add("duration", this.CalculateDuration(lastSuccessfulReport));
            output.Add("metrics", this.SerializeMetrics());

            return output;
        }

        private IDictionary<string, object> SerializeMetrics() 
        {
            IDictionary<string, object> output = new Dictionary<string, object>();

            foreach (MetricData metric in _metrics.Values)
            {
                output.Add(metric.FullName, metric.Serialize());
            }

            return output;
        }

        private int CalculateDuration(DateTime lastSuccessfulReport)
        {  
            return Convert.ToInt32((DateTime.Now - lastSuccessfulReport).TotalSeconds);
        }
    }
}
