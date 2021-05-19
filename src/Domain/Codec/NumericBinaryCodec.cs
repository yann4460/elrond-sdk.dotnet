using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using dotnetstandard_bip32;

namespace Elrond.Dotnet.Sdk.Domain.Codec
{
    public class NumericBinaryCodec : IBinaryCodec<NumericValue>
    {
        public (NumericValue Value, int BytesLength) DecodeNested(byte[] data, TypeValue type = null)
        {
            if (type.HasFixedSize())
            {
                const int offset = 0;
                var length = type.SizeInBytes();
                var payload = data.Slice(offset, offset + length);
                var result = DecodeTopLevel(payload, type);

                var decodedLength = length + offset;
                return (result, decodedLength);
            }
            else
            {
                var sizeInBytes = (int)BitConverter.ToUInt32(data.Slice(0, 4));
                if (BitConverter.IsLittleEndian)
                {
                    sizeInBytes = (int)BitConverter.ToUInt32(data.Slice(0, 4).Reverse().ToArray());
                }

                var payload = data.Skip(4).ToArray();
                var bigNumber = new BigInteger(payload, !type.HasSign(), isBigEndian: true);
                return (new NumericValue(type, bigNumber), sizeInBytes);
            }
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
                var sizeInBytes = value.Type.SizeInBytes();
                var number = value.ValueOf();
                var fullArray = Enumerable.Repeat((byte) 0x00, sizeInBytes).ToArray();
                if (number.IsZero)
                {
                    return fullArray;
                }

                var bigNumber = value.ValueOf();
                var payload = bigNumber.ToByteArray(!value.Type.HasSign(), true);
                var payloadLength = payload.Length;

                var buffer = new List<byte>();
                buffer.AddRange(fullArray.Slice(0, sizeInBytes - payloadLength));
                buffer.AddRange(payload);
                var data = buffer.ToArray();
                return data;
            }
            else
            {
                var payload = EncodeTopLevel(value);
                var sizeBytes = BitConverter.GetBytes(payload.Length).ToList();
                if (BitConverter.IsLittleEndian)
                {
                    sizeBytes.Reverse();
                }

                var buffer = new List<byte>();
                buffer.AddRange(sizeBytes);
                buffer.AddRange(payload);
                var data = buffer.ToArray();
                return data;
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
            var buffer = bigNumber.ToByteArray(isUnsigned, isBigEndian: true);

            return buffer;
        }
    }
}