using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NewRelic.Platform.Sdk.UnitTests
{
    [TestClass]
    public class AgentTest
    {
        [TestMethod]
        public void TestValidPollCycle()
        {
            var agent = new TestAgent("TestAgent");
            var context = new MockContext();
            agent.PrepareToRun(context);
            agent.PollCycle();

            var componentsList = TestSerializationHelper.GetComponentsListFromRequestData(context.RequestData);
            Assert.AreEqual(1, componentsList.Count);

            var componentMap = TestSerializationHelper.GetComponentMapFromComponentsList(componentsList, "TestAgent");
            Assert.AreEqual("TestAgent", componentMap["name"]);

            var metrics = TestSerializationHelper.GetMetricsMapFromComponentMap(componentMap);

            Assert.AreEqual(1, metrics.Count);
        }

        [TestMethod]
        public void TestMultipleValidPollCycle()
        {
            var component = new TestAgent("TestAgent");
            var context = new MockContext();
            component.PrepareToRun(context);

            for (int i = 0; i < 3; i++)
            {
                component.PollCycle();
            }

            var componentsList = TestSerializationHelper.GetComponentsListFromRequestData(context.RequestData);
            Assert.AreEqual(1, componentsList.Count);

            var componentMap = TestSerializationHelper.GetComponentMapFromComponentsList(componentsList, "TestAgent");
            Assert.AreEqual("TestAgent", componentMap["name"]);

            var metrics = TestSerializationHelper.GetMetricsMapFromComponentMap(componentMap);
            Assert.AreEqual(1, metrics.Count);
            Assert.AreEqual(3, TestSerializationHelper.GetValueFromMetricMap(metrics, "Component/Category/TestMetric[unit]", MetricValues.Count));
        }
    }
}
