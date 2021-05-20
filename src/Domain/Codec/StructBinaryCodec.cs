﻿using System;
using System.Collections.Generic;
using System.Linq;
using Elrond.Dotnet.Sdk.Domain.Values;

namespace Elrond.Dotnet.Sdk.Domain.Codec
{
    public class StructBinaryCodec : IBinaryCodec
    {
        private readonly BinaryCodec _binaryCodec;
        public string Type => TypeValue.BinaryTypes.Struct;

        public StructBinaryCodec()
        {
            _binaryCodec = new BinaryCodec();
        }

        public (IBinaryType Value, int BytesLength) DecodeNested(byte[] data, TypeValue type)
        {
            var fieldDefinitions = type.GetFieldDefinitions();
            var fields = new List<StructField>();
            var buffer = data.ToList();
            var offset = 0;
            foreach (var fieldDefinition in fieldDefinitions)
            {
                var fieldType = TypeValue.FromRustType(fieldDefinition.RustType);
                var (value, bytesLength) = _binaryCodec.DecodeNested(buffer.ToArray(), fieldType);
                fields.Add(new StructField(value, fieldDefinition.Name));
                offset += bytesLength;
                buffer = data.Skip(offset).ToList();
            }

            var structObject = new StructValue(type, fields.ToArray());

            return (structObject, offset);
        }

        public IBinaryType DecodeTopLevel(byte[] data, TypeValue type)
        {
            var decoded = this.DecodeNested(data, type);
            return decoded.Value;
        }

        public byte[] EncodeNested(IBinaryType value)
        {
            throw new NotImplementedException();
        }

        public byte[] EncodeTopLevel(IBinaryType value)
        {
            throw new NotImplementedException();
        }
    }
}