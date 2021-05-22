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

        public static OptionValue NewProvided(TypeValue innerType, IBinaryType value)
        {
            return new OptionValue(TypeValue.OptionValue(innerType), innerType, value);
        }

        public bool IsSet()
        {
            return Value != null;
        }


        public override string ToString()
        {
            return IsSet() ? Value.ToString() : "";
        }
    }
}