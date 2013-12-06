using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NewRelic.Platform.Sdk.Binding;
using NewRelic.Platform.Sdk.UnitTests;

namespace NewRelic.Platform.Sdk.UnitTests.Binding
{
    [TestClass]
    public class ComponentDataTest
    {
        [TestMethod]
        public void TestAddSingleMetricSucceeds()
        {
            var component = new ComponentData("TestName", "com.newrelic.test");
            var metric = new MetricData("Test/Metric", "units", 3);

            Assert.AreEqual("TestName", component.Name);
            Assert.AreEqual("com.newrelic.test", component.Guid);
            Assert.AreEqual(0, TestSerializationHelper.GetMetricsMapFromComponent(component).Count, "Should be zero metrics"); 

            component.AddMetric(metric);

            var serializedComponent = component.Serialize(DateTime.Now.Subtract(TimeSpan.FromSeconds(60)));
            var metrics = TestSerializationHelper.GetMetricsMapFromComponent(component);
            Assert.AreEqual(1, metrics.Count, "Should be one metric"); 
            Assert.IsTrue(metrics.Keys.Any(key => key == metric.FullName), "Metric name should be a valid key");
            Assert.AreEqual(3, TestSerializationHelper.GetValueFromMetricMap(metrics, metric.FullName, MetricValues.Value));
        }

        [TestMethod]
        public void TestAddMultipleMetricsSucceeds()
        {
            var component = new ComponentData("TestName", "com.newrelic.test");
            var metric1 = new MetricData("Test/Metric1", "units", 2);
            var metric2 = new MetricData("Test/Metric2", "units", 3);
            var metric3 = new MetricData("Test/Metric3", "units", 4);

            Assert.AreEqual(0, TestSerializationHelper.GetMetricsMapFromComponent(component).Count, "Should be zero metrics");

            component.AddMetric(metric1);
            component.AddMetric(metric2);
            component.AddMetric(metric3);

            var serializedComponent = component.Serialize(DateTime.Now.Subtract(TimeSpan.FromSeconds(60)));
            var metrics = TestSerializationHelper.GetMetricsMapFromComponent(component);
            Assert.AreEqual(3, metrics.Count, "Should be three metrics");

            Assert.AreEqual(2, TestSerializationHelper.GetValueFromMetricMap(metrics, metric1.FullName, MetricValues.Value));
            Assert.AreEqual(3, TestSerializationHelper.GetValueFromMetricMap(metrics, metric2.FullName, MetricValues.Value));
            Assert.AreEqual(4, TestSerializationHelper.GetValueFromMetricMap(metrics, metric3.FullName, MetricValues.Value));
        }

        [TestMethod]
        public void TestAggregatingMetricsSucceeds()
        {
            var component = new ComponentData("TestName", "com.newrelic.test");
            var metric1 = new MetricData("Test/Metric", "units", 2);
            var metric2 = new MetricData("Test/Metric", "units", 3);
            var metric3 = new MetricData("Test/Metric", "units", 4);
            var metric4 = new MetricData("Test/Metric", "otherunits", 5);

            Assert.AreEqual(0, TestSerializationHelper.GetMetricsMapFromComponent(component).Count, "Should be zero metrics");

            component.AddMetric(metric1);
            component.AddMetric(metric2);
            component.AddMetric(metric3);
            component.AddMetric(metric4);

            var serializedComponent = component.Serialize(DateTime.Now.Subtract(TimeSpan.FromSeconds(60)));
            var metrics = TestSerializationHelper.GetMetricsMapFromComponent(component);
            Assert.AreEqual(2, metrics.Count, "Should be two metrics"); 
            Assert.AreEqual(9, TestSerializationHelper.GetValueFromMetricMap(metrics, metric1.FullName, MetricValues.Value));
            Assert.AreEqual(3, TestSerializationHelper.GetValueFromMetricMap(metrics, metric1.FullName, MetricValues.Count));
            Assert.AreEqual(2, TestSerializationHelper.GetValueFromMetricMap(metrics, metric1.FullName, MetricValues.MinValue));
            Assert.AreEqual(4, TestSerializationHelper.GetValueFromMetricMap(metrics, metric1.FullName, MetricValues.MaxValue));
            Assert.AreEqual(29, TestSerializationHelper.GetValueFromMetricMap(metrics, metric1.FullName, MetricValues.SumOfSquares));

            Assert.AreEqual(5, TestSerializationHelper.GetValueFromMetricMap(metrics, metric4.FullName, MetricValues.Value));
            Assert.AreEqual(1, TestSerializationHelper.GetValueFromMetricMap(metrics, metric4.FullName, MetricValues.Count));
            Assert.AreEqual(5, TestSerializationHelper.GetValueFromMetricMap(metrics, metric4.FullName, MetricValues.MinValue));
            Assert.AreEqual(5, TestSerializationHelper.GetValueFromMetricMap(metrics, metric4.FullName, MetricValues.MaxValue));
            Assert.AreEqual(25, TestSerializationHelper.GetValueFromMetricMap(metrics, metric4.FullName, MetricValues.SumOfSquares));
        }
    }
}
