using Erdcsharp.Domain.Exceptions;
using Erdcsharp.Domain.Values;

namespace Erdcsharp.Domain.Codec
{
    public class BooleanBinaryCodec : IBinaryCodec
    {
        public string Type => TypeValue.BinaryTypes.Boolean;

        private const byte True  = 0x01;
        private const byte False = 0x00;

        public (IBinaryType Value, int BytesLength) DecodeNested(byte[] data, TypeValue type)
        {
            // We don't check the size of the buffer, we just read the first byte.
            var firstByte = data[0];
            return (new BooleanValue(firstByte == True), 1);
        }

        public IBinaryType DecodeTopLevel(byte[] data, TypeValue type)
        {
            if (data.Length > 1)
            {
                throw new BinaryCodecException("should be a buffer of size <= 1");
            }

            var firstByte = data[0];
            return new BooleanValue(firstByte == True);
        }

        public byte[] EncodeNested(IBinaryType value)
        {
            var boolean = value.ValueOf<BooleanValue>();
            return boolean.IsTrue() ? new[] {True} : new[] {False};
        }

        public byte[] EncodeTopLevel(IBinaryType value)
        {
            var boolean = value.ValueOf<BooleanValue>();
            return boolean.IsTrue() ? new[] {True} : new byte[0];
        }
    }
}
