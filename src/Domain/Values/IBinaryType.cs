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
            if (string.IsNullOrEmpty(Type.Name))
            {
                var kv = new KeyValuePair<string, string>(Type.Name ?? "", ToString());
                var json = JsonSerializer.Serialize(kv);
                return json;
            }
            else
            {
                var kv = new Dictionary<string, string>
                {
                    {Type.Name, ToString()}
                };
                var json = JsonSerializer.Serialize(kv);
                return json;
            }
        }
    }
}