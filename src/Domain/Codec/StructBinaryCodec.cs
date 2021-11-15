using System.Collections.Generic;
using System.Linq;
using Erdcsharp.Domain.Values;

namespace Erdcsharp.Domain.Codec
{
    public class StructBinaryCodec : IBinaryCodec
    {
        private readonly BinaryCodec _binaryCodec;
        public           string      Type => TypeValue.BinaryTypes.Struct;

        public StructBinaryCodec(BinaryCodec binaryCodec)
        {
            _binaryCodec = binaryCodec;
        }

        public (IBinaryType Value, int BytesLength) DecodeNested(byte[] data, TypeValue type)
        {
            var fieldDefinitions = type.GetFieldDefinitions();
            var fields           = new List<StructField>();
            var buffer           = data.ToList();
            var offset           = 0;
            foreach (var fieldDefinition in fieldDefinitions)
            {
                var (value, bytesLength) = _binaryCodec.DecodeNested(buffer.ToArray(), fieldDefinition.Type);
                fields.Add(new StructField(fieldDefinition.Name, value));

                offset += bytesLength;
                buffer =  buffer.Skip(bytesLength).ToList();
            }

            var structObject = new StructValue(type, fields.ToArray());

            return (structObject, offset);
        }

        public IBinaryType DecodeTopLevel(byte[] data, TypeValue type)
        {
            var decoded = DecodeNested(data, type);
            return decoded.Value;
        }

        public byte[] EncodeNested(IBinaryType value)
        {
            var structValue = value.ValueOf<StructValue>();
            var buffers     = new List<byte[]>();
            var fields      = structValue.Fields;

            foreach (var field in fields)
            {
                var fieldBuffer = _binaryCodec.EncodeNested(field.Value);
                buffers.Add(fieldBuffer);
            }

            var data = buffers.SelectMany(s => s);

            return data.ToArray();
        }

        public byte[] EncodeTopLevel(IBinaryType value)
        {
            return EncodeNested(value);
        }
    }
}
