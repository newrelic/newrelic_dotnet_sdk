using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NewRelic.Platform.Sdk.Binding;

namespace NewRelic.Platform.Sdk.UnitTests.Binding
{
    [TestClass]
    public class MetricDataTest
    {
        [TestMethod]
        public void TestOverloadedConstructorSucceeds()
        {
            var metric = new MetricData("Test/Metric", "units", 2);

            Assert.AreEqual("Component/Test/Metric[units]", metric.FullName);
            Assert.AreEqual(2, metric.Value);
            Assert.AreEqual(1, metric.Count);
            Assert.AreEqual(2, metric.MinValue);
            Assert.AreEqual(2, metric.MaxValue);
            Assert.AreEqual(4, metric.SumOfSquares);
        }

        [TestMethod]
        public void TestAggregationSucceeds()
        {
            var metric1 = new MetricData("Test/Metric1", "units", 2);
            var metric2 = new MetricData("Test/Metric2", "units", 3);

            metric1.AggregateWith(metric2);

            Assert.AreEqual("Component/Test/Metric1[units]", metric1.FullName);
            Assert.AreEqual(5, metric1.Value);
            Assert.AreEqual(2, metric1.Count);
            Assert.AreEqual(2, metric1.MinValue);
            Assert.AreEqual(3, metric1.MaxValue);
            Assert.AreEqual(13, metric1.SumOfSquares);
        }

        [TestMethod]
        public void TestSerializeMethod()
        {
            var metric = new MetricData("Test/Metric", "units", 3);
            var value = metric.Serialize();

            Assert.IsTrue(value is Array);
            Assert.AreEqual((float)value.GetValue(0), metric.Value);
            Assert.AreEqual((float)value.GetValue(1), metric.Count);
            Assert.AreEqual((float)value.GetValue(2), metric.MinValue);
            Assert.AreEqual((float)value.GetValue(3), metric.MaxValue);
            Assert.AreEqual((float)value.GetValue(4), metric.SumOfSquares);
        }
    }
}
