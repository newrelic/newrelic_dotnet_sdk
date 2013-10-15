using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NewRelic.Platform.Binding.DotNET
{
	[DataContract]
	public class ComponentData
	{
		#region Public Properties

		[DataMember(Name = "metrics", Order = 4)]
		public Dictionary<string, object> Metrics { get; private set; }

		private bool HasDataToPost { get; set; }

		[DataMember(Name = "name", Order = 1)]
		public string Name { get; set; }

		[DataMember(Name = "guid", Order = 2)]
		public string Guid { get; set; }

		[DataMember(Name = "duration", Order = 3)]
		public int Duration { get; set; }

		#endregion

		public ComponentData(string name, string guid, int duration)
			: this()
		{
			Name = name;
			Guid = guid;
			Duration = duration;
		}

		public ComponentData()
		{
			Metrics = new Dictionary<string, object>();
			HasDataToPost = false;
		}

		#region Public Methods

		public void AddMetric(string name, int value)
		{
			Metrics[name] = value;
			HasDataToPost = true;
		}

		public void AddMetric(string name, decimal value)
		{
			Metrics[name] = value;
			HasDataToPost = true;
		}

		public void AddMetric(string name, MinMaxMetricValue value)
		{
			Metrics[name] = value;
			HasDataToPost = true;
		}

		public void Clear()
		{
			Metrics.Clear();
		}

		#endregion
	}
}