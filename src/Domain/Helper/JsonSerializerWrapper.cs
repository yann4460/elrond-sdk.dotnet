using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Erdcsharp.Domain.Helper
{
    public class JsonSerializerWrapper
    {
        public static string Serialize(object value)
        {
            var serializerSettings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore, ContractResolver = new CamelCasePropertyNamesContractResolver()};
            return JsonConvert.SerializeObject(value, serializerSettings);
        }

        public static TValue Deserialize<TValue>(string json)
        {
            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            var serializerSettings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore, ContractResolver = new CamelCasePropertyNamesContractResolver()};
            return JsonConvert.DeserializeObject<TValue>(json, serializerSettings);
        }
    }
}
