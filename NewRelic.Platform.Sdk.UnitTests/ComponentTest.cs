using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NewRelic.Platform.Sdk.UnitTests
{
    [TestClass]
    public class ComponentTest
    {
        [TestMethod]
        public void TestValidPollCycle()
        {
            var component = new TestComponent("TestComponent");
            var context = new MockContext();
            component.PrepareToRun(context);
            component.PollCycle();

            var componentsList = TestSerializationHelper.GetComponentsListFromAgent(context.AgentData);
            Assert.AreEqual(1, componentsList.Count);

            var componentMap = TestSerializationHelper.GetComponentMapFromComponentsList(componentsList, "TestComponent");
            Assert.AreEqual("TestComponent", componentMap["name"]);

            var metrics = TestSerializationHelper.GetMetricsMapFromComponentMap(componentMap);

            Assert.AreEqual(1, metrics.Count);
        }

        [TestMethod]
        public void TestMultipleValidPollCycle()
        {
            var component = new TestComponent("TestComponent");
            var context = new MockContext();
            component.PrepareToRun(context);

            for (int i = 0; i < 3; i++)
            {
                component.PollCycle();
            }

            var componentsList = TestSerializationHelper.GetComponentsListFromAgent(context.AgentData);
            Assert.AreEqual(1, componentsList.Count);

            var componentMap = TestSerializationHelper.GetComponentMapFromComponentsList(componentsList, "TestComponent");
            Assert.AreEqual("TestComponent", componentMap["name"]);

            var metrics = TestSerializationHelper.GetMetricsMapFromComponentMap(componentMap);
            Assert.AreEqual(1, metrics.Count);
            Assert.AreEqual(3, TestSerializationHelper.GetValueFromMetricMap(metrics, "Component/Category/TestMetric[unit]", MetricValues.Count));
        }
    }
}
