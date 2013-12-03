using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NewRelic.Platform.Sdk.Binding;

namespace NewRelic.Platform.Sdk.UnitTests.Binding
{
    [TestClass]
    public class PlatformDataTest
    {
        [TestMethod]
        public void TestAddSingleMetricSucceeds()
        {
            var platformData = new PlatformData();

            Assert.AreEqual(0, TestSerializationHelper.GetComponentsListFromAgent(platformData).Count);

            platformData.AddMetric("com.newrelic.test", "TestComponent", "Test/Metric", "unit", 2);

            var componentsList = TestSerializationHelper.GetComponentsListFromAgent(platformData);
            Assert.AreEqual(1, componentsList.Count);

            var component = TestSerializationHelper.GetComponentMapFromComponentsList(componentsList, "TestComponent");
            Assert.AreEqual("com.newrelic.test", component["guid"]);
            Assert.AreEqual("TestComponent", component["name"]);

            var metrics = TestSerializationHelper.GetMetricsMapFromComponentMap(component);

            Assert.AreEqual(1, metrics.Count);

            AssertMetricValues(metrics, "Component/Test/Metric[unit]", 2, 1, 2, 2, 4);
        }

        [TestMethod]
        public void TestAddMultipleMetricsSucceeds()
        {
            var platformData = new PlatformData();

            Assert.AreEqual(0, TestSerializationHelper.GetComponentsListFromAgent(platformData).Count);

            platformData.AddMetric("com.newrelic.test", "TestComponent", "Test/Metric1", "unit", 2);
            platformData.AddMetric("com.newrelic.test", "TestComponent", "Test/Metric2", "unit", 3);
            platformData.AddMetric("com.newrelic.test", "TestComponent", "Test/Metric3", "unit", 4);

            var componentsList = TestSerializationHelper.GetComponentsListFromAgent(platformData);
            Assert.AreEqual(1, componentsList.Count);

            var component = TestSerializationHelper.GetComponentMapFromComponentsList(componentsList, "TestComponent");
            var metrics = TestSerializationHelper.GetMetricsMapFromComponentMap(component);

            Assert.AreEqual(3, metrics.Count);
            AssertMetricValues(metrics, "Component/Test/Metric1[unit]", 2, 1, 2, 2, 4);
            AssertMetricValues(metrics, "Component/Test/Metric3[unit]", 4, 1, 4, 4, 16);
        }

        [TestMethod]
        public void TestAggregateMetricsSucceeds()
        {
            var platformData = new PlatformData();

            Assert.AreEqual(0, TestSerializationHelper.GetComponentsListFromAgent(platformData).Count);

            platformData.AddMetric("com.newrelic.test", "TestComponent", "Test/Metric", "unit", 2);
            platformData.AddMetric("com.newrelic.test", "TestComponent", "Test/Metric", "unit", 3);
            platformData.AddMetric("com.newrelic.test", "TestComponent", "Test/Metric", "unit", 4);

            var componentsList = TestSerializationHelper.GetComponentsListFromAgent(platformData);
            Assert.AreEqual(1, componentsList.Count);

            var component = TestSerializationHelper.GetComponentMapFromComponentsList(componentsList, "TestComponent");
            var metrics = TestSerializationHelper.GetMetricsMapFromComponentMap(component);

            Assert.AreEqual(1, metrics.Count);
            AssertMetricValues(metrics, "Component/Test/Metric[unit]", 9, 3, 2, 4, 29);
        }

        [TestMethod]
        public void TestMultipleComponentsSucceeds()
        {
            var platformData = new PlatformData();

            Assert.AreEqual(0, TestSerializationHelper.GetComponentsListFromAgent(platformData).Count);

            platformData.AddMetric("com.newrelic.test", "TestComponent1", "Test/Metric", "unit", 2);
            platformData.AddMetric("com.newrelic.test", "TestComponent2", "Test/Metric", "unit", 3);
            platformData.AddMetric("com.newrelic.test", "TestComponent3", "Test/Metric", "unit", 4);

            var componentsList = TestSerializationHelper.GetComponentsListFromAgent(platformData);
            Assert.AreEqual(3, componentsList.Count);

            var component1 = TestSerializationHelper.GetComponentMapFromComponentsList(componentsList, "TestComponent1");
            var component2 = TestSerializationHelper.GetComponentMapFromComponentsList(componentsList, "TestComponent2");
            var component3 = TestSerializationHelper.GetComponentMapFromComponentsList(componentsList, "TestComponent3");

            var metrics1 = TestSerializationHelper.GetMetricsMapFromComponentMap(component1);
            var metrics2 = TestSerializationHelper.GetMetricsMapFromComponentMap(component2);
            var metrics3 = TestSerializationHelper.GetMetricsMapFromComponentMap(component3);

            Assert.AreEqual(1, metrics1.Count);
            Assert.AreEqual(1, metrics2.Count);
            Assert.AreEqual(1, metrics3.Count);
        }

        [TestMethod]
        public void TestAggregateWithMultipleComponentsSucceeds()
        {
            var platformData = new PlatformData();

            Assert.AreEqual(0, TestSerializationHelper.GetComponentsListFromAgent(platformData).Count);

            platformData.AddMetric("com.newrelic.test", "TestComponent1", "Test/Metric", "unit", 2);
            platformData.AddMetric("com.newrelic.test", "TestComponent1", "Test/Metric", "unit", 2);

            platformData.AddMetric("com.newrelic.test", "TestComponent2", "Test/Metric", "unit", 3);
            platformData.AddMetric("com.newrelic.test", "TestComponent2", "Test/Metric", "unit", 3);

            platformData.AddMetric("com.newrelic.test", "TestComponent3", "Test/Metric", "unit", 4);
            platformData.AddMetric("com.newrelic.test", "TestComponent3", "Test/Metric", "unit", 4);

            var componentsList = TestSerializationHelper.GetComponentsListFromAgent(platformData);
            Assert.AreEqual(3, componentsList.Count);

            var component1 = TestSerializationHelper.GetComponentMapFromComponentsList(componentsList, "TestComponent1");
            var component2 = TestSerializationHelper.GetComponentMapFromComponentsList(componentsList, "TestComponent2");
            var component3 = TestSerializationHelper.GetComponentMapFromComponentsList(componentsList, "TestComponent3");

            var metrics1 = TestSerializationHelper.GetMetricsMapFromComponentMap(component1);
            var metrics2 = TestSerializationHelper.GetMetricsMapFromComponentMap(component2);
            var metrics3 = TestSerializationHelper.GetMetricsMapFromComponentMap(component3);

            Assert.AreEqual(1, metrics1.Count);
            Assert.AreEqual(1, metrics2.Count);
            Assert.AreEqual(1, metrics3.Count);

            AssertMetricValues(metrics2, "Component/Test/Metric[unit]", 6, 2, 3, 3, 18);
        }

        [TestMethod]
        public void TestResetComponentsSucceeds()
        {
            var platformData = new PlatformData();

            platformData.AddMetric("com.newrelic.test", "TestComponent1", "Test/Metric", "unit", 2);
            platformData.AddMetric("com.newrelic.test", "TestComponent2", "Test/Metric", "unit", 3);
            platformData.AddMetric("com.newrelic.test", "TestComponent3", "Test/Metric", "unit", 4);

            var beforeResetList = TestSerializationHelper.GetComponentsListFromAgent(platformData);
            Assert.AreEqual(3, beforeResetList.Count);

            platformData.Reset();

            var firstAfterResetList = TestSerializationHelper.GetComponentsListFromAgent(platformData);
            Assert.AreEqual(0, firstAfterResetList.Count);

            platformData.AddMetric("com.newrelic.test", "TestComponent1", "Test/Metric", "unit", 2);
            platformData.AddMetric("com.newrelic.test", "TestComponent2", "Test/Metric", "unit", 3);
            platformData.AddMetric("com.newrelic.test", "TestComponent3", "Test/Metric", "unit", 4);

            var secondAfterResetList = TestSerializationHelper.GetComponentsListFromAgent(platformData);
            Assert.AreEqual(3, secondAfterResetList.Count);
        }

        [TestMethod]
        public void TestDifferentGuidDoesntAggregate()
        {
            var platformData = new PlatformData();

            platformData.AddMetric("com.newrelic.test", "TestComponent", "Test/Metric", "unit", 2);
            platformData.AddMetric("com.newrelic.anothertest", "TestComponent", "Test/Metric", "unit", 2);

            var componentsList = TestSerializationHelper.GetComponentsListFromAgent(platformData);
            Assert.AreEqual(2, componentsList.Count);
        }

        [TestMethod]
        public void TestAggregationLimitIsRespected()
        {
            var platformData = new PlatformData();
            platformData.SetAggregationLimit(DateTime.Now.AddMinutes(-20));

            Assert.IsTrue(platformData.IsPastAggregationLimit(), "Aggregation limit is not being respected");
        }

        private void AssertMetricValues(IDictionary<string, object> metrics, string metricName, float value, int count, float min, float max, float sumOfSquares) 
        {
            Assert.AreEqual(value, TestSerializationHelper.GetValueFromMetricMap(metrics, metricName, MetricValues.Value));
            Assert.AreEqual(count, TestSerializationHelper.GetValueFromMetricMap(metrics, metricName, MetricValues.Count));
            Assert.AreEqual(min, TestSerializationHelper.GetValueFromMetricMap(metrics, metricName, MetricValues.MinValue));
            Assert.AreEqual(max, TestSerializationHelper.GetValueFromMetricMap(metrics, metricName, MetricValues.MaxValue));
            Assert.AreEqual(sumOfSquares, TestSerializationHelper.GetValueFromMetricMap(metrics, metricName, MetricValues.SumOfSquares));
        }
    }
}
