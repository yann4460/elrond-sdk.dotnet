using System.Collections.Generic;
using Erdcsharp.Domain.Helper;

namespace Erdcsharp.Domain.Values
{
    public class BaseBinaryValue : IBinaryType
    {
        public TypeValue Type { get; }

        public BaseBinaryValue(TypeValue type)
        {
            Type = type;
        }

        public T ValueOf<T>() where T : IBinaryType
        {
            return (T)(IBinaryType)this;
        }

        public virtual T ToObject<T>()
        {
            return JsonSerializerWrapper.Deserialize<T>(ToJson());
        }

        public virtual string ToJson()
        {
            if (string.IsNullOrEmpty(Type.Name))
            {
                var kv   = new KeyValuePair<string, string>(Type.Name ?? "", ToString());
                var json = JsonSerializerWrapper.Serialize(kv);
                return json;
            }
            else
            {
                var kv   = new Dictionary<string, string> {{Type.Name, ToString()}};
                var json = JsonSerializerWrapper.Serialize(kv);
                return json;
            }
        }
    }
}
