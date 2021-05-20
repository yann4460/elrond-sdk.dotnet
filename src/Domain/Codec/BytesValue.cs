namespace Elrond.Dotnet.Sdk.Domain.Codec
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

        public IBinaryType ValueOf()
        {
            return this;
        }
    }
}