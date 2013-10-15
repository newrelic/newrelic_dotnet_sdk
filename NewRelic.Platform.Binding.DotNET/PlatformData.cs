using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NewRelic.Platform.Binding.DotNET
{
	[DataContract]
	public class PlatformData
	{
		public PlatformData(AgentData agentData)
		{
			AgentData = agentData;
			Components = new List<ComponentData>();
		}

		[DataMember(Name = "agent")]
		public AgentData AgentData { get; set; }

		[DataMember(Name = "components")]
		public List<ComponentData> Components { get; private set; }

		public void AddComponent(ComponentData componentData)
		{
			Components.Add(componentData);
		}
	}
}