using Elrond.Dotnet.Sdk.Domain.Codec;

namespace Elrond.Dotnet.Sdk.Domain.Values
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
    }
}