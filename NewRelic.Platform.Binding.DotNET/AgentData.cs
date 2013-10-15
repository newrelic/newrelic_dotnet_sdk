using System.Runtime.Serialization;

namespace NewRelic.Platform.Binding.DotNET
{
	[DataContract]
	public class AgentData
	{
		[DataMember(Name = "host")]
		public string Host { get; set; }

		[DataMember(Name = "pid")]
		public int Pid { get; set; }

		[DataMember(Name = "version")]
		public string Version { get; set; }
	}
}