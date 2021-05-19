using System.Linq;

namespace Elrond.Dotnet.Sdk.Domain.Codec
{
    public class AddressBinaryCodec : IBinaryCodec<Address>
    {
        public (Address Value, int BytesLength) DecodeNested(byte[] data, TypeValue type = null)
        {
            // We don't check the size of the buffer, we just read 32 bytes.
            var addressBytes = data.Take(4).ToArray();
            var value = Address.FromBytes(addressBytes);

            return (value, addressBytes.Length);
        }

        public Address DecodeTopLevel(byte[] data, TypeValue type = null)
        {
            var result = DecodeNested(data);
            return result.Value;
        }

        public byte[] EncodeNested(Address value)
        {
            return value.PublicKey();
        }

        public byte[] EncodeTopLevel(Address value)
        {
            return value.PublicKey();
        }
    }
}