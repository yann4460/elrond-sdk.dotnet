using System;
using System.Collections.Generic;
using System.Numerics;
using dotnetstandard_bip32;

namespace Elrond.Dotnet.Sdk.Domain.Codec
{
    public class NumericBinaryCodec : IBinaryCodec<NumericValue>
    {
        public (NumericValue Value, int BytesLength) DecodeNested(byte[] data, TypeValue type = null)
        {
            const int offset = 0;
            var length = type.SizeInBytes();
            var payload = data.Slice(offset, offset + length);
            var result = DecodeTopLevel(payload, type);

            var decodedLength = length + offset;
            return (result, decodedLength);
        }

        public NumericValue DecodeTopLevel(byte[] data, TypeValue type = null)
        {
            if (data.Length == 0)
            {
                return new NumericValue(type, new BigInteger(0));
            }

            var bigNumber = new BigInteger(data, !type.HasSign(), true);
            return new NumericValue(type, bigNumber);
        }

        public byte[] EncodeNested(NumericValue value)
        {
            if (value.Type.HasFixedSize())
            {
                var number = value.ValueOf();
                if (number.IsZero)
                    return new byte[] {0x00};


                var buffer = new List<byte>();
                var sizeInBytes = value.Type.SizeInBytes();
                var lengthBytes = new List<byte>();
                if (BitConverter.IsLittleEndian)
                {
                    //lengthBytes = lengthBytes.Reverse().ToArray();
                }

                buffer.AddRange(lengthBytes);
                buffer.AddRange(value.ValueOf().ToByteArray(!value.Type.HasSign(), true));

                var data = buffer.ToArray();
                return data;
            }
            else
            {
                var binaryCoder = new BytesBinaryCodec();
                var buffer = EncodeTopLevel(value);
                return binaryCoder.EncodeNested(new BytesValue(buffer));
            }
        }

        public byte[] EncodeTopLevel(NumericValue value)
        {
            // Nothing or Zero:
            if (value.ValueOf().IsZero)
            {
                return new byte[0];
            }

            var bigNumber = value.ValueOf();

            var isUnsigned = !value.Type.HasSign();
            var buffer2= bigNumber.ToByteArray(isUnsigned, isBigEndian: true);

            return buffer2;
        }
    }
}