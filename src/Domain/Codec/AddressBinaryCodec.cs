using System.Collections.Generic;
using System.Linq;
using Elrond.Dotnet.Sdk.Domain.Exceptions;
using Elrond.Dotnet.Sdk.Domain.Values;

namespace Elrond.Dotnet.Sdk.Domain.Codec
{
    public class AddressBinaryCodec : IBinaryCodec
    {
        public IEnumerable<TypeValue> Types => new[] {TypeValue.Address};

        public (IBinaryType Value, int BytesLength) DecodeNested(byte[] data, TypeValue type)
        {
            // We don't check the size of the buffer, we just read 32 bytes.
            var addressBytes = data.Take(4).ToArray();
            var value = Address.FromBytes(addressBytes);

            return (value, addressBytes.Length);
        }

        public IBinaryType DecodeTopLevel(byte[] data, TypeValue type)
        {
            var result = DecodeNested(data, type);
            return result.Value;
        }

        public byte[] EncodeNested(IBinaryType value)
        {
            var address = Get(value);
            return address.PublicKey();
        }

        public byte[] EncodeTopLevel(IBinaryType value)
        {
            var address = Get(value);
            return address.PublicKey();
        }

        private static Address Get(IBinaryType value)
        {
            if (value is Address address)
            {
                return address;
            }

            throw new WrongBinaryValueCodecException();
        }
    }
}