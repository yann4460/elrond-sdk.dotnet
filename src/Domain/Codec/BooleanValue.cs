namespace Elrond.Dotnet.Sdk.Domain.Codec
{
    public class BooleanValue : IBinaryType
    {
        private readonly bool _value;

        public BooleanValue(bool value)
        {
            _value = value;
        }

        public bool IsTrue()
        {
            return _value == true;
        }

        public bool IsFalse()
        {
            return _value == false;
        }

        public TypeValue Type => TypeValue.Boolean;

        public IBinaryType ValueOf()
        {
            return this;
        }
    }
}