namespace Elrond.Dotnet.Sdk.Domain.Codec
{
    public class BytesValue : PrimitiveValue
    {
        private readonly byte[] _data;

        public BytesValue(byte[] data)
        {
            _data = data;
        }

        public int GetLength()
        {
            return _data.Length;
        }

        public byte[] ValueOf()
        {
            return _data;
        }
    }
}