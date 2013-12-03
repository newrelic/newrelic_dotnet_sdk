using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using NewRelic.Platform.Sdk.Binding;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NewRelic.Platform.Sdk.Utils
{
    public static class JsonHelper
    {
        /// <summary>
        /// Serializes a list of simple .NET types (e.g. IDictionary, List, and primitives) into a JSON string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize(object obj) 
        {
            return JsonConvert.SerializeObject(obj, new KeyValuePairConverter());
        }

        /// <summary>
        /// Deserializes a JSON into a list of simple .NET types (e.g. IDictionary, List, and primitives)
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static object Deserialize(string json)
        {
            return JsonToDotNetType(JToken.Parse(json));
        }

        private static object JsonToDotNetType(JToken token)
        {
            if (token.Type == JTokenType.Object)
            {
                IDictionary<string, object> dict = new Dictionary<string, object>();
                foreach (JProperty prop in ((JObject)token).Properties())
                {
                    dict.Add(prop.Name, JsonToDotNetType(prop.Value));
                }
                return dict;
            }
            else if (token.Type == JTokenType.Array)
            {
                List<object> list = new List<object>();
                foreach (JToken value in token)
                {
                    list.Add(JsonToDotNetType(value));
                }
                return list;
            }
            else
            {
                return ((JValue)token).Value;
            }
        }
    }
}
