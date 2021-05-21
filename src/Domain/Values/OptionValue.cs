namespace Elrond.Dotnet.Sdk.Domain.Values
{
    public class OptionValue : IBinaryType
    {
        public TypeValue Type { get; }
        public TypeValue InnerType { get; }
        public IBinaryType Value { get; }

        private OptionValue(TypeValue type, TypeValue innerType, IBinaryType value = null)
        {
            Type = type;
            InnerType = innerType;
            Value = value;
        }

        public static OptionValue NewMissing(TypeValue innerType)
        {
            return new OptionValue(TypeValue.OptionValue(innerType), innerType);
        }

        public static OptionValue NewProvided(TypeValue innerType, IBinaryType value)
        {
            return new OptionValue(TypeValue.OptionValue(innerType), innerType, value);
        }

        public bool IsSet()
        {
            return Value != null;
        }
    }
}