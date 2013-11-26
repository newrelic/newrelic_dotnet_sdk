using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NewRelic.Platform.Sdk.Binding;

namespace NewRelic.Platform.Sdk.UnitTests.Binding
{
    [TestClass]
    public class AgentDataTest
    {
        [TestMethod]
        public void TestAddSingleMetricSucceeds()
        {
            var agent = new AgentData();

            Assert.AreEqual(0, TestSerializationHelper.GetComponentsListFromAgent(agent).Count);

            agent.AddMetric("com.newrelic.test", "TestComponent", "Test/Metric", "unit", 2);

            var componentsList = TestSerializationHelper.GetComponentsListFromAgent(agent);
            Assert.AreEqual(1, componentsList.Count);

            var component = TestSerializationHelper.GetComponentMapFromComponentsList(componentsList, "TestComponent");
            Assert.AreEqual("com.newrelic.test", component["guid"]);
            Assert.AreEqual("TestComponent", component["name"]);

            var metrics = TestSerializationHelper.GetMetricsMapFromComponentMap(component);

            Assert.AreEqual(1, metrics.Count);
            Assert.AreEqual(2, TestSerializationHelper.GetValueFromMetricMap(metrics, "Component/Test/Metric[unit]", MetricValues.Value));
            Assert.AreEqual(1, TestSerializationHelper.GetValueFromMetricMap(metrics, "Component/Test/Metric[unit]", MetricValues.Count));
            Assert.AreEqual(2, TestSerializationHelper.GetValueFromMetricMap(metrics, "Component/Test/Metric[unit]", MetricValues.MinValue));
            Assert.AreEqual(2, TestSerializationHelper.GetValueFromMetricMap(metrics, "Component/Test/Metric[unit]", MetricValues.MaxValue));
            Assert.AreEqual(4, TestSerializationHelper.GetValueFromMetricMap(metrics, "Component/Test/Metric[unit]", MetricValues.SumOfSquares));
        }

        [TestMethod]
        public void TestAddMultipleMetricsSucceeds()
        {
            var agent = new AgentData();

            Assert.AreEqual(0, TestSerializationHelper.GetComponentsListFromAgent(agent).Count);

            agent.AddMetric("com.newrelic.test", "TestComponent", "Test/Metric1", "unit", 2);
            agent.AddMetric("com.newrelic.test", "TestComponent", "Test/Metric2", "unit", 3);
            agent.AddMetric("com.newrelic.test", "TestComponent", "Test/Metric3", "unit", 4);

            var componentsList = TestSerializationHelper.GetComponentsListFromAgent(agent);
            Assert.AreEqual(1, componentsList.Count);

            var component = TestSerializationHelper.GetComponentMapFromComponentsList(componentsList, "TestComponent");
            var metrics = TestSerializationHelper.GetMetricsMapFromComponentMap(component);

            Assert.AreEqual(3, metrics.Count);
            Assert.AreEqual(2, TestSerializationHelper.GetValueFromMetricMap(metrics, "Component/Test/Metric1[unit]", MetricValues.Value));
            Assert.AreEqual(1, TestSerializationHelper.GetValueFromMetricMap(metrics, "Component/Test/Metric1[unit]", MetricValues.Count));
            Assert.AreEqual(2, TestSerializationHelper.GetValueFromMetricMap(metrics, "Component/Test/Metric1[unit]", MetricValues.MinValue));
            Assert.AreEqual(2, TestSerializationHelper.GetValueFromMetricMap(metrics, "Component/Test/Metric1[unit]", MetricValues.MaxValue));
            Assert.AreEqual(4, TestSerializationHelper.GetValueFromMetricMap(metrics, "Component/Test/Metric1[unit]", MetricValues.SumOfSquares));

            Assert.AreEqual(4, TestSerializationHelper.GetValueFromMetricMap(metrics, "Component/Test/Metric3[unit]", MetricValues.Value));
            Assert.AreEqual(1, TestSerializationHelper.GetValueFromMetricMap(metrics, "Component/Test/Metric3[unit]", MetricValues.Count));
            Assert.AreEqual(4, TestSerializationHelper.GetValueFromMetricMap(metrics, "Component/Test/Metric3[unit]", MetricValues.MinValue));
            Assert.AreEqual(4, TestSerializationHelper.GetValueFromMetricMap(metrics, "Component/Test/Metric3[unit]", MetricValues.MaxValue));
            Assert.AreEqual(16, TestSerializationHelper.GetValueFromMetricMap(metrics, "Component/Test/Metric3[unit]", MetricValues.SumOfSquares));
        }

        [TestMethod]
        public void TestAggregateMetricsSucceeds()
        {
            var agent = new AgentData();

            Assert.AreEqual(0, TestSerializationHelper.GetComponentsListFromAgent(agent).Count);

            agent.AddMetric("com.newrelic.test", "TestComponent", "Test/Metric", "unit", 2);
            agent.AddMetric("com.newrelic.test", "TestComponent", "Test/Metric", "unit", 3);
            agent.AddMetric("com.newrelic.test", "TestComponent", "Test/Metric", "unit", 4);

            var componentsList = TestSerializationHelper.GetComponentsListFromAgent(agent);
            Assert.AreEqual(1, componentsList.Count);

            var component = TestSerializationHelper.GetComponentMapFromComponentsList(componentsList, "TestComponent");
            var metrics = TestSerializationHelper.GetMetricsMapFromComponentMap(component);

            Assert.AreEqual(1, metrics.Count);
            Assert.AreEqual(9, TestSerializationHelper.GetValueFromMetricMap(metrics, "Component/Test/Metric[unit]", MetricValues.Value));
            Assert.AreEqual(3, TestSerializationHelper.GetValueFromMetricMap(metrics, "Component/Test/Metric[unit]", MetricValues.Count));
            Assert.AreEqual(2, TestSerializationHelper.GetValueFromMetricMap(metrics, "Component/Test/Metric[unit]", MetricValues.MinValue));
            Assert.AreEqual(4, TestSerializationHelper.GetValueFromMetricMap(metrics, "Component/Test/Metric[unit]", MetricValues.MaxValue));
            Assert.AreEqual(29, TestSerializationHelper.GetValueFromMetricMap(metrics, "Component/Test/Metric[unit]", MetricValues.SumOfSquares));
        }

        [TestMethod]
        public void TestMultipleComponentsSucceeds()
        {
            var agent = new AgentData();

            Assert.AreEqual(0, TestSerializationHelper.GetComponentsListFromAgent(agent).Count);

            agent.AddMetric("com.newrelic.test", "TestComponent1", "Test/Metric", "unit", 2);
            agent.AddMetric("com.newrelic.test", "TestComponent2", "Test/Metric", "unit", 3);
            agent.AddMetric("com.newrelic.test", "TestComponent3", "Test/Metric", "unit", 4);

            var componentsList = TestSerializationHelper.GetComponentsListFromAgent(agent);
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
            var agent = new AgentData();

            Assert.AreEqual(0, TestSerializationHelper.GetComponentsListFromAgent(agent).Count);

            agent.AddMetric("com.newrelic.test", "TestComponent1", "Test/Metric", "unit", 2);
            agent.AddMetric("com.newrelic.test", "TestComponent1", "Test/Metric", "unit", 2);

            agent.AddMetric("com.newrelic.test", "TestComponent2", "Test/Metric", "unit", 3);
            agent.AddMetric("com.newrelic.test", "TestComponent2", "Test/Metric", "unit", 3);

            agent.AddMetric("com.newrelic.test", "TestComponent3", "Test/Metric", "unit", 4);
            agent.AddMetric("com.newrelic.test", "TestComponent3", "Test/Metric", "unit", 4);

            var componentsList = TestSerializationHelper.GetComponentsListFromAgent(agent);
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

            Assert.AreEqual(6, TestSerializationHelper.GetValueFromMetricMap(metrics2, "Component/Test/Metric[unit]", MetricValues.Value));
            Assert.AreEqual(2, TestSerializationHelper.GetValueFromMetricMap(metrics2, "Component/Test/Metric[unit]", MetricValues.Count));
            Assert.AreEqual(3, TestSerializationHelper.GetValueFromMetricMap(metrics2, "Component/Test/Metric[unit]", MetricValues.MinValue));
            Assert.AreEqual(3, TestSerializationHelper.GetValueFromMetricMap(metrics2, "Component/Test/Metric[unit]", MetricValues.MaxValue));
            Assert.AreEqual(18, TestSerializationHelper.GetValueFromMetricMap(metrics2, "Component/Test/Metric[unit]", MetricValues.SumOfSquares));
        }

        [TestMethod]
        public void TestResetComponentsSucceeds()
        {
            var agent = new AgentData();

            agent.AddMetric("com.newrelic.test", "TestComponent1", "Test/Metric", "unit", 2);
            agent.AddMetric("com.newrelic.test", "TestComponent2", "Test/Metric", "unit", 3);
            agent.AddMetric("com.newrelic.test", "TestComponent3", "Test/Metric", "unit", 4);

            var beforeResetList = TestSerializationHelper.GetComponentsListFromAgent(agent);
            Assert.AreEqual(3, beforeResetList.Count);

            agent.Reset();

            var firstAfterResetList = TestSerializationHelper.GetComponentsListFromAgent(agent);
            Assert.AreEqual(0, firstAfterResetList.Count);

            agent.AddMetric("com.newrelic.test", "TestComponent1", "Test/Metric", "unit", 2);
            agent.AddMetric("com.newrelic.test", "TestComponent2", "Test/Metric", "unit", 3);
            agent.AddMetric("com.newrelic.test", "TestComponent3", "Test/Metric", "unit", 4);

            var secondAfterResetList = TestSerializationHelper.GetComponentsListFromAgent(agent);
            Assert.AreEqual(3, secondAfterResetList.Count);
        }

        [TestMethod]
        public void TestAggregationLimitIsRespected()
        {
            var agent = new AgentData();
            agent.SetAggregationLimit(DateTime.Now.AddMinutes(-20));

            Assert.IsTrue(agent.IsPastAggregationLimit(), "Aggregation limit is not being respected");
        }
    }
}
