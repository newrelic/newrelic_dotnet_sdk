using System.Collections.Generic;
using System.Diagnostics;

namespace NewRelic.Platform.Sdk.Binding
{
    internal class AgentData
    {
        private string _host = System.Environment.MachineName;
        private int _pid = Process.GetCurrentProcess().Id;

        internal string Version { get; set; }

        internal AgentData()
        {
        }

        internal IDictionary<string, object> Serialize()
        {
            IDictionary<string, object> output = new Dictionary<string, object>();
            output.Add("host", _host);
            output.Add("version", this.Version);
            output.Add("pid", _pid);
            return output;
        }
    }
}
