using System.Collections.Generic;
using System.Text.Json;

namespace Elrond.Dotnet.Sdk.Domain.Values
{
    public interface IBinaryType
    {
        TypeValue Type { get; }

        T ValueOf<T>() where T : IBinaryType
        {
            return (T) this;
        }

        public T ToObject<T>()
        {
            var json = ToJSON();
            return JsonSerializer.Deserialize<T>(json);
        }

        string ToJSON()
        {
            var option = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
            if (string.IsNullOrEmpty(Type.Name))
            {
                var kv = new KeyValuePair<string, string>(Type.Name ?? "", ToString());
                var json = JsonSerializer.Serialize(kv, option);
                return json;
            }
            else
            {
                var kv = new Dictionary<string, string>
                {
                    {Type.Name, ToString()}
                };
                var json = JsonSerializer.Serialize(kv, option);
                return json;
            }
        }
    }
}