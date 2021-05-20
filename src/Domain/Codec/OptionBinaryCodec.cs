using System.Collections.Generic;
using dotnetstandard_bip32;
using Elrond.Dotnet.Sdk.Domain.Exceptions;
using Elrond.Dotnet.Sdk.Domain.Values;

namespace Elrond.Dotnet.Sdk.Domain.Codec
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
                return (OptionValue.NewMissing(type), 1);
            }

            if (data[0] != 0x01)
            {
                throw new BinaryCodecException("invalid buffer for optional value");
            }

            var (value, bytesLength) = _binaryCodec.DecodeNested(data.Slice(1), type);
            return (OptionValue.NewProvided(type, value), bytesLength + 1);
        }

        public IBinaryType DecodeTopLevel(byte[] data, TypeValue type)
        {
            if (data.Length == 0)
            {
                return OptionValue.NewMissing(type);
            }

            if (data[0] != 0x01)
            {
                throw new BinaryCodecException("invalid buffer for optional value");
            }

            var (value, _) = _binaryCodec.DecodeNested(data.Slice(1), type);
            return OptionValue.NewProvided(type, value);
        }

        public byte[] EncodeNested(IBinaryType value)
        {
            var optionValue = value.ValueOf<OptionValue>();
            if (optionValue.IsSet())
            {
                var encoded = _binaryCodec.EncodeNested(optionValue.Value, optionValue.InnerType);
                var payload = new List<byte> { 0x01 };
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