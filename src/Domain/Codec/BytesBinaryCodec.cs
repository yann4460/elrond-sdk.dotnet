using System;
using System.Collections.Generic;
using System.Linq;
using dotnetstandard_bip32;

namespace Elrond.Dotnet.Sdk.Domain.Codec
{
    public class BytesBinaryCodec : IBinaryCodec
    {
        public IEnumerable<TypeValue> Types => new[] {TypeValue.Bytes};

        private const int BytesSizeOfU32 = 4;

        public (IBinaryType Value, int BytesLength) DecodeNested(byte[] data, TypeValue type)
        {
            var sizeInBytes = BitConverter.ToUInt32(data);
            if (BitConverter.IsLittleEndian)
            {
                sizeInBytes = BitConverter.ToUInt32(BitConverter.GetBytes(sizeInBytes).Reverse().ToArray());
            }

            var payload = data.Slice(BytesSizeOfU32, BytesSizeOfU32 + (int) sizeInBytes);

            return (new BytesValue(payload), payload.Length);
        }

        public IBinaryType DecodeTopLevel(byte[] data, TypeValue type)
        {
            return new BytesValue(data);
        }

        public byte[] EncodeNested(IBinaryType value)
        {
            var bytes = value.ValueOf() as BytesValue;
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
            var bytes = value.ValueOf() as BytesValue;
            return bytes.Buffer;
        }
    }
}