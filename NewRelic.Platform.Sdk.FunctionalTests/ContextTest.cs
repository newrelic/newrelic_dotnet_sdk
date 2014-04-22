using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NewRelic.Platform.Sdk.Binding;
using System.Net;
using NewRelic.Platform.Sdk.Utils;

namespace NewRelic.Platform.Sdk.FunctionalTests
{
    [TestClass]
    [DeploymentItem(@"config/newrelic.json", "config")]
    [DeploymentItem(@"config/plugin.json", "config")]
    public class ContextTest
    {
        [TestMethod]
        public void TestSendMetricsToServiceSucceeds()
        {
            var context = new Context() { Version = "1.0.0" };

            context.ReportMetric("com.newrelic.sdkfunctest", "FunctionalTest1", "TestMetric", "unit", 2);
            context.ReportMetric("com.newrelic.sdkfunctest", "FunctionalTest1", "TestMetric", "unit", 3);
            context.ReportMetric("com.newrelic.sdkfunctest", "FunctionalTest1", "TestMetric", "unit", 4);

            context.ReportMetric("com.newrelic.sdkfunctest", "FunctionalTest2", "TestMetric", "unit", 5);

            context.ReportMetric("com.newrelic.sdkfunctest", "FunctionalTest3", "TestMetric", "unit", 6);
            context.ReportMetric("com.newrelic.sdkfunctest", "FunctionalTest3", "TestMetric", "unit", 7);

            var requestData = context.RequestData;
            Assert.IsTrue(requestData.HasComponents(), "Request data should have components before send");
            Assert.AreEqual(3, ((List<object>)requestData.Serialize()["components"]).Count, "There should be three components present");

            // Will throw an exception for any client errors (400-499)
            context.SendMetricsToService();

            requestData = context.RequestData;
            Assert.IsFalse(requestData.HasComponents(), "Request data should be cleared after a successful send");
        }

        [TestMethod]
        public void TestSendMetricsToServiceFailsWithNoVersion()
        {
            var context = new Context();
            context.ReportMetric("com.newrelic.sdkfunctest", "FunctionalTest", "TestMetric", "unit", 2);

            try
            {
                context.SendMetricsToService();
                Assert.Fail("Exception should be thrown when no version is set");
            }
            catch (NewRelicServiceException nrse)
            {
                Assert.AreEqual(HttpStatusCode.BadRequest, nrse.StatusCode, "Service should respond with 400 when no version is set");
            }
        }

        [TestMethod]
        public void TestSendMetricsToServiceFailsWithBadLicense()
        {
            var context = new Context("GarbageLicense") { Version = "1.0.0" };
            context.ReportMetric("com.newrelic.sdkfunctest", "FunctionalTest", "TestMetric", "unit", 2);

            try
            {
                context.SendMetricsToService();
                Assert.Fail("Exception should be thrown when no version is set");
            }
            catch (NewRelicServiceException nrse)
            {
                Assert.AreEqual(HttpStatusCode.Forbidden, nrse.StatusCode, "Service should respond with 403 when invalid license key is sent");
            }
        }
    }
}
