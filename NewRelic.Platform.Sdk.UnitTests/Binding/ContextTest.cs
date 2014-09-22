using NewRelic.Platform.Sdk.Binding;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;

namespace NewRelic.Platform.Sdk.UnitTests
{
    [TestClass]
    public class ContextTest
    {
        [TestMethod]
        public void TestContextInitialization()
        {
            var context = new Context();

            Assert.IsNotNull(context.ServiceUri, "The initialized context does not have a default service URI");
        }

        [TestMethod]
        public void TestContextInitializationWithLicenseKey()
        {
            string license = "FakeyMcFakeKey";
            var context = new Context(license);

            Assert.AreEqual(license, context.LicenseKey);
        }

        [TestMethod]
        public void TestReportValidMetric()
        {
            var context = new Context();
            context.ReportMetric("com.newrelic.test", "TestComponent", "Category/TestMetric", "unit", 5); // Assert no throws
        }

        [TestMethod]
        public void TestReportBadMetricNullGuid()
        {
            try
            {
                var context = new Context();
                context.ReportMetric("", "TestComponent", "Category/TestMetric", "unit", 2);
                Assert.Fail("Reporting metric with null guid should fail");
            }
            catch (ArgumentNullException)
            {
                // Expected
            }
        }

        [TestMethod]
        public void TestReportBadMetricEmptyComponent()
        {
            try
            {
                var context = new Context();
                context.ReportMetric("com.newrelic.test", "", "Category/TestMetric", "unit", 2);
                Assert.Fail("Reporting metric with empty component should fail");
            }
            catch (ArgumentNullException)
            {
                // Expected
            }
        }

        [TestMethod]
        public void TestReportBadMetricNegativeValue()
        {
            try
            {
                var context = new Context();
                context.ReportMetric("com.newrelic.test", "TestComponent", "Category/TestMetric", "unit", -2);
                Assert.Fail("Reporting metric with null guid should fail");
            }
            catch (ArgumentException)
            {
                // Expected
            }
        }
    }
}
