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

        string ToJSON()
        {
            var kv = new KeyValuePair<string, string>(Type.Name, ToString());
            return JsonSerializer.Serialize(kv);
        }
    }
}