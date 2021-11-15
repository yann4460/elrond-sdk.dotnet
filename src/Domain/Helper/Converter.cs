using System;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Erdcsharp.Domain.Helper
{
    public static class Converter
    {
        private const byte UnsignedByte = 0x00;

        public static BigInteger ToBigInteger(byte[] bytes, bool isUnsigned = false, bool isBigEndian = false)
        {
            if (isUnsigned)
            {
                if (bytes.FirstOrDefault() != UnsignedByte)
                {
                    var data = new[] {UnsignedByte}.ToList();
                    data.AddRange(bytes);
                    bytes = data.ToArray();
                }
            }

            if (isBigEndian)
            {
                bytes = bytes.Reverse().ToArray();
            }

            return new BigInteger(bytes);
        }

        public static byte[] FromBigInteger(BigInteger bigInteger, bool isUnsigned = false, bool isBigEndian = false)
        {
            var bytes = bigInteger.ToByteArray();
            if (isBigEndian)
            {
                bytes = bytes.Reverse().ToArray();
            }

            if (!isUnsigned)
                return bytes;

            if (bytes.FirstOrDefault() == UnsignedByte)
            {
                bytes = bytes.Slice(1);
            }

            return bytes;
        }

        public static string ToHexString(byte[] bytes)
        {
            var hex = BitConverter
                     .ToString(bytes)
                     .Replace("-", "");

            return hex;
        }

        public static byte[] FromHexString(string hex)
        {
            var bytes    = new byte[hex.Length / 2];
            var hexValue = new[] {0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F};

            for (int x = 0, i = 0; i < hex.Length; i += 2, x += 1)
            {
                bytes[x] = (byte)(hexValue[char.ToUpper(hex[i + 0]) - '0'] << 4 |
                                  hexValue[char.ToUpper(hex[i + 1]) - '0']);
            }

            return bytes;
        }

        public static string ToHexString(string utf8Value)
        {
            return ToHexString(Encoding.UTF8.GetBytes(utf8Value));
        }

        public static string ToBase64String(string value)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
        }
    }
}
