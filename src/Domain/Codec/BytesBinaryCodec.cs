using System;
using System.Collections.Generic;
using System.Linq;
using dotnetstandard_bip32;
using Elrond.Dotnet.Sdk.Domain.Values;

namespace Elrond.Dotnet.Sdk.Domain.Codec
{
    public class BytesBinaryCodec : IBinaryCodec
    {
        public string Type => TypeValue.BinaryTypes.Bytes;

        private const int BytesSizeOfU32 = 4;

        public (IBinaryType Value, int BytesLength) DecodeNested(byte[] data, TypeValue type)
        {
            var sizeInBytes = BitConverter.ToUInt32(data.Take(BytesSizeOfU32).ToArray());
            if (BitConverter.IsLittleEndian)
            {
                sizeInBytes = BitConverter.ToUInt32(BitConverter.GetBytes(sizeInBytes).Reverse().ToArray());
            }

            var payload = data.Slice(BytesSizeOfU32, BytesSizeOfU32 + (int) sizeInBytes);

            return (new BytesValue(payload, type), payload.Length);
        }

        public IBinaryType DecodeTopLevel(byte[] data, TypeValue type)
        {
            return new BytesValue(data, type);
        }

        public byte[] EncodeNested(IBinaryType value)
        {
            var bytes = value.ValueOf<BytesValue>();
            var buffer = new List<byte>();
            var lengthBytes = BitConverter.GetBytes(bytes.GetLength());
            if (BitConverter.IsLittleEndian)
            {
                lengthBytes = lengthBytes.Reverse().ToArray();
            }

            buffer.AddRange(lengthBytes);
            buffer.AddRange(bytes.Buffer);

            var data = buffer.ToArray();

            return data;
        }

        public byte[] EncodeTopLevel(IBinaryType value)
        {
            var bytes = value.ValueOf<BytesValue>();
            return bytes.Buffer;
        }
    }
}