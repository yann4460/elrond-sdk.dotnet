namespace Elrond.Dotnet.Sdk.Domain.Values
{
    public class BooleanValue : IBinaryType
    {
        public TypeValue Type => TypeValue.BooleanValue;

        private readonly bool _value;
        public byte[] Buffer { get; }
        public BooleanValue(bool value)
        {
            _value = value;
        }

        public static BooleanValue From(bool value)
        {
            return new BooleanValue(value);
        }

        public bool IsTrue()
        {
            return _value;
        }

        public bool IsFalse()
        {
            return _value == false;
        }

    }
}