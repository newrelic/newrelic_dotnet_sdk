using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NewRelic.Platform.Sdk.Processors;

namespace NewRelic.Platform.Sdk.UnitTests.Processors
{
    [TestClass]
    public class EpochProcessorTest
    {
        [TestMethod]
        public void TestTwoValidPollCycles()
        {
            var processor = new EpochProcessor();
            var firstProcess = processor.Process(5);
            ResetTimer(processor, 1);
            var secondProcess = processor.Process(6);

            Assert.IsNull(firstProcess);
            Assert.AreEqual(1.0, Convert.ToDouble(secondProcess.Value), 0.1);
        }

        [TestMethod]
        public void TestNullSecondPollCycle()
        {
            var processor = new EpochProcessor();
            var firstProcess = processor.Process(5);
            ResetTimer(processor, 1);
            var secondProcess = processor.Process(null);
            ResetTimer(processor, 1);
            var thirdProcess = processor.Process(6);
            ResetTimer(processor, 1);
            var fourthProcess = processor.Process(7);

            Assert.IsNull(firstProcess);
            Assert.IsNull(secondProcess);
            Assert.IsNull(thirdProcess);
            Assert.AreEqual(1.0, Convert.ToDouble(fourthProcess.Value), 0.1);
        }

        [TestMethod]
        public void TestNullFirstPollCycle()
        {
            var processor = new EpochProcessor();
            var firstProcess = processor.Process(null);
            ResetTimer(processor, 1);
            var secondProcess = processor.Process(5);
            ResetTimer(processor, 1);
            var thirdProcess = processor.Process(6);

            Assert.IsNull(firstProcess);
            Assert.IsNull(secondProcess);
            Assert.AreEqual(1.0, Convert.ToDouble(thirdProcess.Value), 0.1);
        }

        private void ResetTimer(EpochProcessor processor, int seconds)
        {
            processor.LastValue = DateTime.Now.Subtract(TimeSpan.FromSeconds(seconds));
        }
    }
}
