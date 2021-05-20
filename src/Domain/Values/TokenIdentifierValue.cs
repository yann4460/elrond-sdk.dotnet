namespace Elrond.Dotnet.Sdk.Domain.Values
{
    public class TokenIdentifierValue : IBinaryType
    {
        public TokenIdentifierValue(byte[] data, TypeValue type)
        {
            Buffer = data;
            Type = type;
        }

        public int GetLength()
        {
            return Buffer.Length;
        }

        public TypeValue Type { get; }

        public byte[] Buffer { get; }
    }
}