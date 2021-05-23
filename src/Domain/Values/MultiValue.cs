using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Elrond.Dotnet.Sdk.Domain.Values
{
    public class MultiValue : IBinaryType
    {
        public TypeValue Type { get; }
        public Dictionary<TypeValue, IBinaryType> Values { get; }

        public MultiValue(TypeValue type, Dictionary<TypeValue, IBinaryType> values)
        {
            Type = type;
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
            foreach (var (key, value) in Values)
            {
                builder.AppendLine($"{key}:{value}");
            }

            return builder.ToString();
        }

        public string ToJSON()
        {
            var dic = new Dictionary<string, object>();
            for (var i = 0; i < Values.Count; i++)
            {
                var value = Values.ToArray()[i];
                if (value.Value.Type.BinaryType == TypeValue.BinaryTypes.Struct)
                {
                    var json = value.Value.ToJSON();
                    var jsonObject = JsonSerializer.Deserialize<dynamic>(json);
                    dic.Add($"Multi_{i}", jsonObject);
                }
                else
                {
                    dic.Add($"Multi_{i}", value.Value.ToString());
                }
            }

            return JsonSerializer.Serialize(dic, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });
        }
    }
}