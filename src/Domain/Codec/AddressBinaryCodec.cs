using System.Linq;
using Elrond.Dotnet.Sdk.Domain.Exceptions;
using Elrond.Dotnet.Sdk.Domain.Values;

namespace Elrond.Dotnet.Sdk.Domain.Codec
{
    public class AddressBinaryCodec : IBinaryCodec
    {
        public string Type => TypeValue.BinaryTypes.Address;

        public (IBinaryType Value, int BytesLength) DecodeNested(byte[] data, TypeValue type)
        {
            // We don't check the size of the buffer, we just read 32 bytes.
            var addressBytes = data.Take(32).ToArray();
            var value = AddressValue.FromBytes(addressBytes);
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

        private static AddressValue Get(IBinaryType value)
        {
            if (value is AddressValue address)
            {
                return address;
            }

            throw new WrongBinaryValueCodecException();
        }
    }
}