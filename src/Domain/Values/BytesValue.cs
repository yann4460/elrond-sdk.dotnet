using System;
using System.Text;

namespace Elrond.Dotnet.Sdk.Domain.Values
{
    public class BytesValue : IBinaryType
    {
        public BytesValue(byte[] data, TypeValue type)
        {
            Buffer = data;
            Type = type;
        }

        public int GetLength()
        {
            return Buffer.Length;
        }

        public static BytesValue FromUtf8(string utf8String)
        {
            return new BytesValue(Encoding.UTF8.GetBytes(utf8String), TypeValue.BytesValue);
        }

        public static BytesValue FromHex(string hexString)
        {
            return new BytesValue(Convert.FromHexString(hexString), TypeValue.BytesValue);
        }

        public static BytesValue FromBuffer(byte[] data)
        {
            return new BytesValue(data, TypeValue.BytesValue);
        }

        public TypeValue Type { get; }

        public byte[] Buffer { get; }

        public override string ToString()
        {
            return Convert.ToHexString(Buffer);
        }
    }
}