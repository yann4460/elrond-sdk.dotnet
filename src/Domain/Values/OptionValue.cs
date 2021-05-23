using System.Text.Json;

namespace Elrond.Dotnet.Sdk.Domain.Values
{
    public class OptionValue : IBinaryType
    {
        public TypeValue Type { get; }
        public TypeValue InnerType { get; }
        public IBinaryType Value { get; }

        private OptionValue(TypeValue type, TypeValue innerType = null, IBinaryType value = null)
        {
            Type = type;
            InnerType = innerType;
            Value = value;
        }

        public static OptionValue NewMissing()
        {
            return new OptionValue(TypeValue.OptionValue());
        }

        public static OptionValue NewProvided(IBinaryType value)
        {
            return new OptionValue(TypeValue.OptionValue(value.Type), value.Type, value);
        }

        public bool IsSet()
        {
            return Value != null;
        }


        public override string ToString()
        {
            return IsSet() ? Value.ToString() : "";
        }

        public T ToObject<T>()
        {
            var json = ToJSON();
            return JsonSerializer.Deserialize<T>(json);
        }

        string ToJSON()
        {
            return IsSet() ? Value.ToJSON() : "{}";
        }
    }
}