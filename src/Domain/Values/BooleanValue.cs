using System;

namespace Elrond.Dotnet.Sdk.Domain.Values
{
    public class BooleanValue : IBinaryType
    {
        private readonly bool _value;
        public byte[] Buffer { get; }

        public BooleanValue(bool value)
        {
            _value = value;
            Buffer = BitConverter.GetBytes(value);
        }

        public bool IsTrue()
        {
            return _value;
        }

        public bool IsFalse()
        {
            return _value == false;
        }

        public TypeValue Type => TypeValue.BooleanValue;
    }
}