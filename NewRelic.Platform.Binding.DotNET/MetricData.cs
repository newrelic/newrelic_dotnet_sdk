using System;
using System.Runtime.Serialization;

using Newtonsoft.Json;

namespace NewRelic.Platform.Binding.DotNET
{
	[JsonConverter(typeof (MinMaxMetricValueJsonSerializer))]
	public class MinMaxMetricValue
	{
		public MinMaxMetricValue(int min, int nax, int total, int count, int sumOfSquares)
		{
			Min = min;
			Max = nax;
			Total = total;
			Count = count;
			SumOfSquares = sumOfSquares;
		}

		[DataMember(Name = "min")]
		public int Min { get; set; }

		[DataMember(Name = "max")]
		public int Max { get; set; }

		[DataMember(Name = "total")]
		public int Total { get; set; }

		[DataMember(Name = "count")]
		public int Count { get; set; }

		[DataMember(Name = "sum_of_squares")]
		public int SumOfSquares { get; set; }
	}

	public class MinMaxMetricValueJsonSerializer : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var objToSerialize = value as MinMaxMetricValue;
			if (objToSerialize != null)
			{
				writer.WriteStartArray();
				writer.WriteValue(objToSerialize.Min);
				writer.WriteValue(objToSerialize.Max);
				writer.WriteValue(objToSerialize.Total);
				writer.WriteValue(objToSerialize.Count);
				writer.WriteValue(objToSerialize.SumOfSquares);
				writer.WriteEndArray();
			}
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override bool CanConvert(Type objectType)
		{
			return typeof (MinMaxMetricValue).IsAssignableFrom(objectType);
		}
	}
}