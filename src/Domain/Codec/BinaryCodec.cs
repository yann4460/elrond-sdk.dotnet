﻿using System.Collections.Generic;
using System.Linq;
using Elrond.Dotnet.Sdk.Domain.Exceptions;
using Elrond.Dotnet.Sdk.Domain.Values;

namespace Elrond.Dotnet.Sdk.Domain.Codec
{
    public class BinaryCodec
    {
        private readonly List<IBinaryCodec> _codecs;

        public BinaryCodec()
        {
            _codecs = new List<IBinaryCodec>
            {
                new NumericBinaryCodec(),
                new AddressBinaryCodec(),
                new BooleanBinaryCodec(),
                new BytesBinaryCodec(),
                new TokenIdentifierCodec(),
                new StructBinaryCodec(this),
                new OptionBinaryCodec(this),
            };
        }

        public (IBinaryType Value, int BytesLength) DecodeNested(byte[] data, TypeValue type)
        {
            CheckBufferLength(data);

            var codec = _codecs.SingleOrDefault(c => c.Type == type.BinaryType);
            var decode = codec.DecodeNested(data, type);
            return decode;
        }

        public IBinaryType DecodeTopLevel(byte[] data, TypeValue type)
        {
            CheckBufferLength(data);

            var codec = _codecs.SingleOrDefault(c => c.Type == type.BinaryType);
            var decode = codec.DecodeTopLevel(data, type);
            return decode;
        }

        public byte[] EncodeNested(IBinaryType value, TypeValue type)
        {
            var codec = _codecs.SingleOrDefault(c => c.Type == type.BinaryType);
            var encode = codec.EncodeNested(value);
            return encode;
        }

        public byte[] EncodeTopLevel(IBinaryType value, TypeValue type)
        {
            var codec = _codecs.SingleOrDefault(c => c.Type == type.BinaryType);
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