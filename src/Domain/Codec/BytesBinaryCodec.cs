using System;
using System.Collections.Generic;
using System.Linq;
using dotnetstandard_bip32;

namespace Elrond.Dotnet.Sdk.Domain.Codec
{
    public class BytesBinaryCodec : IBinaryCodec<BytesValue>
    {
        private const int BytesSizeOfU32 = 4;

        public (BytesValue Value, int BytesLength) DecodeNested(byte[] data)
        {
            var value = BitConverter.ToUInt32(data);
            if (BitConverter.IsLittleEndian)
            {
                var valueBytes = BitConverter.GetBytes(value);
                value = BitConverter.ToUInt32(valueBytes.Reverse().ToArray());
            }

            var payload = data.Slice(BytesSizeOfU32, BytesSizeOfU32 + (int) value);

            return (new BytesValue(payload), payload.Length);
        }

        public BytesValue DecodeTopLevel(byte[] data)
        {
            return new BytesValue(data);
        }

        public byte[] EncodeNested(BytesValue value)
        {
            var buffer = new List<byte>();
            var lengthBytes = BitConverter.GetBytes(value.GetLength());

            if (BitConverter.IsLittleEndian)
            {
                lengthBytes = lengthBytes.Reverse().ToArray();
            }

            buffer.AddRange(lengthBytes);
            buffer.AddRange(value.ValueOf());

            var data = buffer.ToArray();

            return data;
        }

        public byte[] EncodeTopLevel(BytesValue value)
        {
            return value.ValueOf();
        }
    }
}