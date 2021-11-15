using System.Collections.Generic;
using System.Linq;
using System.Text;
using Erdcsharp.Domain.Helper;

namespace Erdcsharp.Domain.Values
{
    public class MultiValue : BaseBinaryValue
    {
        public Dictionary<TypeValue, IBinaryType> Values { get; }

        public MultiValue(TypeValue type, Dictionary<TypeValue, IBinaryType> values) : base(type)
        {
            Values = values;
        }

        public static MultiValue From(params IBinaryType[] values)
        {
            var t = values.Select(s => s.Type).ToArray();
            return new MultiValue(TypeValue.MultiValue(t), values.ToDictionary(s => s.Type, d => d));
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine(Type.Name);
            foreach (var value in Values)
            {
                builder.AppendLine($"{value.Key}:{value}");
            }

            return builder.ToString();
        }

        public override T ToObject<T>()
        {
            return JsonSerializerWrapper.Deserialize<T>(ToJson());
        }

        public override string ToJson()
        {
            var dic = new Dictionary<string, object>();
            for (var i = 0; i < Values.Count; i++)
            {
                var value = Values.ToArray()[i];
                if (value.Value.Type.BinaryType == TypeValue.BinaryTypes.Struct)
                {
                    var json       = value.Value.ToJson();
                    var jsonObject = JsonSerializerWrapper.Deserialize<object>(json);
                    dic.Add($"Multi_{i}", jsonObject);
                }
                else
                {
                    dic.Add($"Multi_{i}", value.Value.ToString());
                }
            }

            return JsonSerializerWrapper.Serialize(dic);
        }
    }
}
