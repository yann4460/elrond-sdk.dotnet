using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Erdcsharp.Domain.Helper;
using Erdcsharp.Domain.Values;

namespace Erdcsharp.Domain.Codec
{
    public class NumericBinaryCodec : IBinaryCodec
    {
        public string Type => TypeValue.BinaryTypes.Numeric;

        public (IBinaryType Value, int BytesLength) DecodeNested(byte[] data, TypeValue type)
        {
            if (type.HasFixedSize())
            {
                var length  = type.SizeInBytes();
                var payload = data.Slice(0, length);
                var result  = DecodeTopLevel(payload, type);
                return (result, length);
            }
            else
            {
                const int u32Size     = 4;
                var       sizeInBytes = (int)BitConverter.ToUInt32(data.Slice(0, u32Size), 0);
                if (BitConverter.IsLittleEndian)
                {
                    sizeInBytes = (int)BitConverter.ToUInt32(data.Slice(0, u32Size).Reverse().ToArray(), 0);
                }

                var payload   = data.Skip(u32Size).Take(sizeInBytes).ToArray();
                var bigNumber = Converter.ToBigInteger(payload, !type.HasSign(), isBigEndian: true);
                return (new NumericValue(type, bigNumber), sizeInBytes + u32Size);
            }
        }

        public IBinaryType DecodeTopLevel(byte[] data, TypeValue type)
        {
            if (data.Length == 0)
            {
                return new NumericValue(type, new BigInteger(0));
            }

            var bigNumber = Converter.ToBigInteger(data, !type.HasSign(), isBigEndian: true);
            return new NumericValue(type, bigNumber);
        }

        public byte[] EncodeNested(IBinaryType value)
        {
            var numericValue = value.ValueOf<NumericValue>();
            if (value.Type.HasFixedSize())
            {
                var sizeInBytes = value.Type.SizeInBytes();
                var number      = numericValue.Number;
                var fullArray   = Enumerable.Repeat((byte)0x00, sizeInBytes).ToArray();
                if (number.IsZero)
                {
                    return fullArray;
                }

                var payload       = Converter.FromBigInteger(number, !value.Type.HasSign(), true);
                var payloadLength = payload.Length;

                var buffer = new List<byte>();
                buffer.AddRange(fullArray.Slice(0, sizeInBytes - payloadLength));
                buffer.AddRange(payload);
                var data = buffer.ToArray();
                return data;
            }
            else
            {
                var payload   = EncodeTopLevel(value);
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

        public byte[] EncodeTopLevel(IBinaryType value)
        {
            var numericValue = value.ValueOf<NumericValue>();
            if (numericValue.Number.IsZero)
            {
                return new byte[0];
            }

            var isUnsigned = !value.Type.HasSign();
            var buffer     = Converter.FromBigInteger(numericValue.Number, isUnsigned, isBigEndian: true);

            return buffer;
        }
    }
}
