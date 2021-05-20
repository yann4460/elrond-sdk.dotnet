using Elrond.Dotnet.Sdk.Domain.Codec;

namespace Elrond.Dotnet.Sdk.Domain.Values
{
    public class BytesValue : IBinaryType
    {
        public BytesValue(byte[] data)
        {
            Buffer = data;
        }

        public int GetLength()
        {
            return Buffer.Length;
        }


        public TypeValue Type => TypeValue.Bytes;

        public byte[] Buffer { get; }
    }
}