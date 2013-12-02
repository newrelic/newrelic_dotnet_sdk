using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NewRelic.Platform.Sdk.Binding;

namespace NewRelic.Platform.Sdk.UnitTests
{
    internal static class TestSerializationHelper
    {
        internal static List<object> GetComponentsListFromAgent(PlatformData platformData)
        {
            return (List<object>)platformData.Serialize()["components"];
        }

        internal static IDictionary<string, object> GetMetricsMapFromComponent(ComponentData component)
        {
            return ((IDictionary<string, object>)component.Serialize(DateTime.Now.Subtract(TimeSpan.FromSeconds(60)))["metrics"]);
        }

        internal static IDictionary<string, object> GetComponentMapFromComponentsList(List<object> components, string name)
        {
            var castedComponents = components.Cast<IDictionary<string, object>>();
            return castedComponents.FirstOrDefault(c => c["name"].ToString() == name);
        }

        internal static IDictionary<string, object> GetMetricsMapFromComponentMap(IDictionary<string, object> component)
        {
            return (IDictionary<string, object>)component["metrics"];
        }

        internal static float GetValueFromMetricMap(IDictionary<string, object> metrics, string name, MetricValues type)
        {
            return (float)((Array)metrics[name]).GetValue((int)type);
        }
    }

    internal enum MetricValues
    {
        Value = 0,
        Count = 1,
        MinValue = 2,
        MaxValue = 3,
        SumOfSquares = 4
    }
}
