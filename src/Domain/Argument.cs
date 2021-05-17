using System;
using System.Text;

namespace Elrond.Dotnet.Sdk.Domain
{
    public class Argument
    {
        private Argument(string hexValue)
        {
            Value = hexValue;
        }

        public string Value { get; }

        public static T GetValue<T>(string value, int index = 0)
        {
            var split = value.Split('@', StringSplitOptions.RemoveEmptyEntries);
            //var base64 = Encoding.UTF8.GetString(Convert.FromBase64String(value));
            var arg = split[index];


            if (typeof(T) == typeof(Address))
            {
                return (T)Convert.ChangeType(Address.FromHex(arg), typeof(Address));
            }

            var type = Type.GetTypeCode(typeof(T));
            switch (type)
            {
                case TypeCode.Boolean:
                    switch (arg)
                    {
                        case "01":
                            return (T)Convert.ChangeType(true, typeof(bool));
                        default:
                            return (T)Convert.ChangeType(false, typeof(bool));
                    }
                case TypeCode.Int16:
                    if (arg == null)
                        return (T)Convert.ChangeType(0, typeof(short));
                    return (T)Convert.ChangeType(Convert.ToInt16(arg, 16), typeof(short));
                case TypeCode.Int32:
                    if (arg == null)
                        return (T)Convert.ChangeType(0, typeof(int));
                    return (T)Convert.ChangeType(Convert.ToInt32(arg, 16), typeof(int));
                case TypeCode.Int64:
                    if (arg == null)
                        return (T)Convert.ChangeType(0, typeof(long));
                    return (T)Convert.ChangeType(Convert.ToInt64(arg, 16), typeof(long));
                case TypeCode.String:
                    var stringValue = Encoding.UTF8.GetString(Convert.FromHexString(arg));
                    return (T)Convert.ChangeType(stringValue, typeof(string));
                case TypeCode.UInt16:
                    if (arg == null)
                        return (T)Convert.ChangeType(0, typeof(ushort));
                    return (T)Convert.ChangeType(Convert.ToUInt16(arg, 16), typeof(ushort));
                case TypeCode.UInt32:
                    if (arg == null)
                        return (T)Convert.ChangeType(0, typeof(uint));
                    return (T)Convert.ChangeType(Convert.ToUInt32(arg, 16), typeof(uint));
                case TypeCode.UInt64:
                    if (arg == null)
                        return (T)Convert.ChangeType(0, typeof(ulong));
                    return (T)Convert.ChangeType(Convert.ToUInt64(arg, 16), typeof(ulong));
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.DateTime:
                default:
                    throw new ArgumentOutOfRangeException(nameof(TypeCode), type, null);
            }
        }

        public static Argument CreateArgumentFromBytes(byte[] bytes)
        {
            var hexEncoded = Convert.ToHexString(bytes);
            return new Argument(hexEncoded);
        }

        public static Argument CreateArgumentFromUtf8String(string utf8String)
        {
            var bytes = Encoding.UTF8.GetBytes(utf8String);
            return CreateArgumentFromBytes(bytes);
        }

        public static Argument CreateArgumentFromBoolean(bool value)
        {
            var bytes = BitConverter.GetBytes(value);
            return CreateArgumentFromBytes(bytes);
        }

        public static Argument CreateArgumentFromHex(string hexValue)
        {
            hexValue = hexValue.Length % 2 == 0 ? hexValue : "0" + hexValue;
            return new Argument(hexValue);
        }

        public static Argument CreateArgumentFromInt16(short value)
        {
            var hex = value.ToString("X");
            return CreateArgumentFromHex(hex);
        }

        public static Argument CreateArgumentFromInt32(int value)
        {
            var hex = value.ToString("X");
            return CreateArgumentFromHex(hex);
        }

        public static Argument CreateArgumentFromInt64(long value)
        {
            var hex = value.ToString("X");
            return CreateArgumentFromHex(hex);
        }
    }
}