using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NewRelic.Platform.Sdk.FunctionalTests
{
    [TestClass]
    [DeploymentItem(@"config/newrelic.json", "config")]
    [DeploymentItem(@"config/plugin.json", "config")]
    public class RunnerTest
    {
        [TestMethod]
        public void TestRunnerSetupAndRunSucceeds()
        {
            Runner runner = new Runner();

            runner.Add(new TestAgentFactory());
            runner.Add(new TestAgent("FunctionalTest3", 4));
            runner.Add(new TestAgent("FunctionalTest4", 5));

            runner.SetupAndRunWithLimit(1);

            Assert.AreEqual(4, runner.Agents.Count);
        }

        [TestMethod]
        public void TestRunnerSetupFailsWithNullAgent()
        {
            try
            {
                Runner runner = new Runner();
                Agent agent = null;
                runner.Add(agent);
                Assert.Fail("Runner should raise exception when null agent is passed");
            }
            catch (ArgumentNullException)
            {
                // Expected
            }
        }

        [TestMethod]
        public void TestRunnerReportContinuesWithNullAgentName()
        {
            Runner runner = new Runner();
            runner.Add(new TestAgent("", 4));
            runner.SetupAndRunWithLimit(1); // Should not raise an exception
        }

        [TestMethod]
        public void TestRunnerReportContinuesWithNegativeValue()
        {
            Runner runner = new Runner();
            runner.Add(new TestAgent("FunctionalTest", -10));
            runner.SetupAndRunWithLimit(1); // Should not raise an exception
        }
    }
}
