using Erdcsharp.Domain.Exceptions;
using Erdcsharp.Domain.Values;

namespace Erdcsharp.Domain.Codec
{
    public class TokenIdentifierCodec : IBinaryCodec
    {
        private readonly BytesBinaryCodec _bytesBinaryCodec;

        public TokenIdentifierCodec()
        {
            _bytesBinaryCodec = new BytesBinaryCodec();
        }

        public string Type => TypeValue.BinaryTypes.TokenIdentifier;

        public (IBinaryType Value, int BytesLength) DecodeNested(byte[] data, TypeValue type)
        {
            var (value, bytesLength) = _bytesBinaryCodec.DecodeNested(data, type);
            return (TokenIdentifierValue.From(value.ValueOf<BytesValue>().Buffer), bytesLength);
        }

        public IBinaryType DecodeTopLevel(byte[] data, TypeValue type)
        {
            var bytesValue = _bytesBinaryCodec.DecodeTopLevel(data, type);
            return TokenIdentifierValue.From(bytesValue.ValueOf<BytesValue>().Buffer);
        }

        public byte[] EncodeNested(IBinaryType value)
        {
            var tokenIdentifierValue = Get(value);
            var byteValue            = new BytesValue(tokenIdentifierValue.Buffer, TypeValue.TokenIdentifierValue);
            return _bytesBinaryCodec.EncodeNested(byteValue);
        }

        public byte[] EncodeTopLevel(IBinaryType value)
        {
            var tokenIdentifierValue = Get(value);
            return tokenIdentifierValue.Buffer;
        }

        private static TokenIdentifierValue Get(IBinaryType value)
        {
            if (value is TokenIdentifierValue tokenIdentifierValue)
            {
                return tokenIdentifierValue;
            }

            throw new WrongBinaryValueCodecException();
        }
    }
}
