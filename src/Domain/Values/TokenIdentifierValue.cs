using System.Text;

namespace Elrond.Dotnet.Sdk.Domain.Values
{
    public class TokenIdentifierValue : IBinaryType
    {
        public TokenIdentifierValue(byte[] data, TypeValue type)
        {
            Buffer = data;
            Type = type;
            TokenName = Encoding.UTF8.GetString(data);
        }

        public string TokenName { get; }
        public TypeValue Type { get; }
        public byte[] Buffer { get; }

        public static TokenIdentifierValue From(byte[] data)
        {
            return new TokenIdentifierValue(data, TypeValue.TokenIdentifierValue);
        }

        public static TokenIdentifierValue EGLD()
        {
            return new TokenIdentifierValue(new byte[0], TypeValue.TokenIdentifierValue);
        }

        public static TokenIdentifierValue From(string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            return new TokenIdentifierValue(bytes, TypeValue.TokenIdentifierValue);
        }
    }
}