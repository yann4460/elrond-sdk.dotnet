using System.Collections.Generic;
using Erdcsharp.Domain.Exceptions;
using Erdcsharp.Domain.Helper;
using Erdcsharp.Domain.Values;

namespace Erdcsharp.Domain.Codec
{
    public class OptionBinaryCodec : IBinaryCodec
    {
        private readonly BinaryCodec _binaryCodec;

        public OptionBinaryCodec(BinaryCodec binaryCodec)
        {
            _binaryCodec = binaryCodec;
        }

        public string Type => TypeValue.BinaryTypes.Option;

        public (IBinaryType Value, int BytesLength) DecodeNested(byte[] data, TypeValue type)
        {
            if (data[0] == 0x00)
            {
                return (OptionValue.NewMissing(), 1);
            }

            if (data[0] != 0x01)
            {
                throw new BinaryCodecException("invalid buffer for optional value");
            }

            var (value, bytesLength) = _binaryCodec.DecodeNested(data.Slice(1), type.InnerType);
            return (OptionValue.NewProvided(value), bytesLength + 1);
        }

        public IBinaryType DecodeTopLevel(byte[] data, TypeValue type)
        {
            if (data.Length == 0)
            {
                return OptionValue.NewMissing();
            }

            var decoded = _binaryCodec.DecodeTopLevel(data, type.InnerType);
            return OptionValue.NewProvided(decoded);
        }

        public byte[] EncodeNested(IBinaryType value)
        {
            var optionValue = value.ValueOf<OptionValue>();
            if (optionValue.IsSet())
            {
                var encoded = _binaryCodec.EncodeNested(optionValue.Value);
                var payload = new List<byte> {0x01};
                payload.AddRange(encoded);
                return payload.ToArray();
            }

            return new byte[] {0x00};
        }

        public byte[] EncodeTopLevel(IBinaryType value)
        {
            var optionValue = value.ValueOf<OptionValue>();
            if (optionValue.IsSet())
                return EncodeNested(value);

            return new byte[] { };
        }
    }
}
