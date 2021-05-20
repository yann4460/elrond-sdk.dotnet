using System.Collections.Generic;
using System.Linq;
using Elrond.Dotnet.Sdk.Domain.Exceptions;

namespace Elrond.Dotnet.Sdk.Domain.Codec
{
    public class BinaryCoder
    {
        private readonly List<IBinaryCodec> _codecs;

        public BinaryCoder()
        {
            _codecs = new List<IBinaryCodec>
            {
                new NumericBinaryCodec(),
                new AddressBinaryCodec(),
                new BooleanBinaryCodec(),
                new BytesBinaryCodec()
            };
        }

        public (IBinaryType Value, int BytesLength) DecodeNested(byte[] data, TypeValue type)
        {
            CheckBufferLength(data);

            var codec = _codecs.SingleOrDefault(c => c.Types.Any(t => t.Name == type.Name));
            var decode = codec.DecodeNested(data, type);
            return decode;
        }

        public IBinaryType DecodeTopLevel(byte[] data, TypeValue type)
        {
            CheckBufferLength(data);

            var codec = _codecs.SingleOrDefault(c => c.Types.Any(t => t.Name == type.Name));
            var decode = codec.DecodeTopLevel(data, type);
            return decode;
        }

        public byte[] EncodeNested(IBinaryType value, TypeValue type)
        {
            var codec = _codecs.SingleOrDefault(c => c.Types.Any(t => t.Name == type.Name));
            var encode = codec.EncodeNested(value);
            return encode;
        }

        public byte[] EncodeTopLevel(IBinaryType value, TypeValue type)
        {
            var codec = _codecs.SingleOrDefault(c => c.Types.Any(t => t.Name == type.Name));
            var encode = codec.EncodeNested(value);
            return encode;
        }


        private void CheckBufferLength(byte[] buffer)
        {
            if (buffer.Length > 4096)
            {
                throw new BinaryCodecException("Buffer too large");
            }
        }
    }
}