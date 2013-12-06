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
    public class RequestDataTest
    {
        [TestMethod]
        public void TestAddSingleMetricSucceeds()
        {
            var requestData = new RequestData();

            Assert.AreEqual(0, TestSerializationHelper.GetComponentsListFromRequestData(requestData).Count);

            requestData.AddMetric("com.newrelic.test", "TestComponent", "Test/Metric", "unit", 2);

            var componentsList = TestSerializationHelper.GetComponentsListFromRequestData(requestData);
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
            var requestData = new RequestData();

            Assert.AreEqual(0, TestSerializationHelper.GetComponentsListFromRequestData(requestData).Count);

            requestData.AddMetric("com.newrelic.test", "TestComponent", "Test/Metric1", "unit", 2);
            requestData.AddMetric("com.newrelic.test", "TestComponent", "Test/Metric2", "unit", 3);
            requestData.AddMetric("com.newrelic.test", "TestComponent", "Test/Metric3", "unit", 4);

            var componentsList = TestSerializationHelper.GetComponentsListFromRequestData(requestData);
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
            var requestData = new RequestData();

            Assert.AreEqual(0, TestSerializationHelper.GetComponentsListFromRequestData(requestData).Count);

            requestData.AddMetric("com.newrelic.test", "TestComponent", "Test/Metric", "unit", 2);
            requestData.AddMetric("com.newrelic.test", "TestComponent", "Test/Metric", "unit", 3);
            requestData.AddMetric("com.newrelic.test", "TestComponent", "Test/Metric", "unit", 4);

            var componentsList = TestSerializationHelper.GetComponentsListFromRequestData(requestData);
            Assert.AreEqual(1, componentsList.Count);

            var component = TestSerializationHelper.GetComponentMapFromComponentsList(componentsList, "TestComponent");
            var metrics = TestSerializationHelper.GetMetricsMapFromComponentMap(component);

            Assert.AreEqual(1, metrics.Count);
            AssertMetricValues(metrics, "Component/Test/Metric[unit]", 9, 3, 2, 4, 29);
        }

        [TestMethod]
        public void TestMultipleComponentsSucceeds()
        {
            var requestData = new RequestData();

            Assert.AreEqual(0, TestSerializationHelper.GetComponentsListFromRequestData(requestData).Count);

            requestData.AddMetric("com.newrelic.test", "TestComponent1", "Test/Metric", "unit", 2);
            requestData.AddMetric("com.newrelic.test", "TestComponent2", "Test/Metric", "unit", 3);
            requestData.AddMetric("com.newrelic.test", "TestComponent3", "Test/Metric", "unit", 4);

            var componentsList = TestSerializationHelper.GetComponentsListFromRequestData(requestData);
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
            var requestData = new RequestData();

            Assert.AreEqual(0, TestSerializationHelper.GetComponentsListFromRequestData(requestData).Count);

            requestData.AddMetric("com.newrelic.test", "TestComponent1", "Test/Metric", "unit", 2);
            requestData.AddMetric("com.newrelic.test", "TestComponent1", "Test/Metric", "unit", 2);

            requestData.AddMetric("com.newrelic.test", "TestComponent2", "Test/Metric", "unit", 3);
            requestData.AddMetric("com.newrelic.test", "TestComponent2", "Test/Metric", "unit", 3);

            requestData.AddMetric("com.newrelic.test", "TestComponent3", "Test/Metric", "unit", 4);
            requestData.AddMetric("com.newrelic.test", "TestComponent3", "Test/Metric", "unit", 4);

            var componentsList = TestSerializationHelper.GetComponentsListFromRequestData(requestData);
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
            var requestData = new RequestData();

            requestData.AddMetric("com.newrelic.test", "TestComponent1", "Test/Metric", "unit", 2);
            requestData.AddMetric("com.newrelic.test", "TestComponent2", "Test/Metric", "unit", 3);
            requestData.AddMetric("com.newrelic.test", "TestComponent3", "Test/Metric", "unit", 4);

            var beforeResetList = TestSerializationHelper.GetComponentsListFromRequestData(requestData);
            Assert.AreEqual(3, beforeResetList.Count);

            requestData.Reset();

            var firstAfterResetList = TestSerializationHelper.GetComponentsListFromRequestData(requestData);
            Assert.AreEqual(0, firstAfterResetList.Count);

            requestData.AddMetric("com.newrelic.test", "TestComponent1", "Test/Metric", "unit", 2);
            requestData.AddMetric("com.newrelic.test", "TestComponent2", "Test/Metric", "unit", 3);
            requestData.AddMetric("com.newrelic.test", "TestComponent3", "Test/Metric", "unit", 4);

            var secondAfterResetList = TestSerializationHelper.GetComponentsListFromRequestData(requestData);
            Assert.AreEqual(3, secondAfterResetList.Count);
        }

        [TestMethod]
        public void TestDifferentGuidDoesntAggregate()
        {
            var requestData = new RequestData();

            requestData.AddMetric("com.newrelic.test", "TestComponent", "Test/Metric", "unit", 2);
            requestData.AddMetric("com.newrelic.anothertest", "TestComponent", "Test/Metric", "unit", 2);

            var componentsList = TestSerializationHelper.GetComponentsListFromRequestData(requestData);
            Assert.AreEqual(2, componentsList.Count);
        }

        [TestMethod]
        public void TestAggregationLimitIsRespected()
        {
            var requestData = new RequestData();
            requestData.SetAggregationLimit(DateTime.Now.AddMinutes(-20));

            Assert.IsTrue(requestData.IsPastAggregationLimit(), "Aggregation limit is not being respected");
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
