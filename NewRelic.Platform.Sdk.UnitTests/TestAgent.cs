namespace NewRelic.Platform.Sdk.UnitTests
{
    internal class TestAgent : Agent
    {
        public override string Guid { get { return "com.newrelic.test"; } }
        public override string Version { get { return "1.0.0"; } }

        private string _name;

        public TestAgent(string name)
        {
            _name = name;
        }

        public override string GetAgentName()
        {
            return _name;
        }

        public override void PollCycle()
        {
            ReportMetric("Category/TestMetric", "unit", 2);
        }
    }
}
