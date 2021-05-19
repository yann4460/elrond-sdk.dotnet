using System;
using Elrond.Dotnet.Sdk.Domain.Exceptions;

namespace Elrond.Dotnet.Sdk.Domain.Codec
{
    public class BooleanBinaryCodec : IBinaryCodec<BooleanValue>
    {
        private const byte True = 0x01;
        private const byte False = 0x00;

        public (BooleanValue Value, int BytesLength) DecodeNested(byte[] data, TypeValue type = null)
        {
            // We don't check the size of the buffer, we just read the first byte.
            var firstByte = data[0];
            return (new BooleanValue(firstByte == True), 1);
        }

        public BooleanValue DecodeTopLevel(byte[] data, TypeValue type = null)
        {
            if (data.Length > 1)
            {
                throw new BinaryCodecException("should be a buffer of size <= 1");
            }

            var firstByte = data[0];
            return new BooleanValue(firstByte == True);
        }


        public byte[] EncodeNested(BooleanValue value)
        {
            return value.IsTrue() ? new[] {True} : new[] {False};
        }

        public byte[] EncodeTopLevel(BooleanValue value)
        {
            return value.IsTrue() ? new[] {True} : new byte[0];
        }
    }

    public class BooleanValue : PrimitiveValue
    {
        private readonly bool _value;

        public BooleanValue(bool value)
        {
            _value = value;
        }

        public bool IsTrue()
        {
            return _value;
        }

        public bool IsFalse()
        {
            return _value;
        }

        public bool ValueOf()
        {
            return _value;
        }
    }
}