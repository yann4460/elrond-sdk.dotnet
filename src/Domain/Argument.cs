using System;
using System.Numerics;
using System.Text;
using Elrond.Dotnet.Sdk.Domain.Codec;

namespace Elrond.Dotnet.Sdk.Domain
{
    public class Argument
    {
        private Argument(string hexValue)
        {
            Value = hexValue.ToLowerInvariant();
        }

        public string Value { get; }

        public static T GetValue<T>(string value, int index = 0)
        {
            var split = value.Split('@', StringSplitOptions.RemoveEmptyEntries);
            var arg = split[index];


            if (typeof(T) == typeof(Address))
            {
                return (T) Convert.ChangeType(Address.FromHex(arg), typeof(Address));
            }

            var type = Type.GetTypeCode(typeof(T));
            switch (type)
            {
                case TypeCode.Boolean:
                    switch (arg)
                    {
                        case "01":
                            return (T) Convert.ChangeType(true, typeof(bool));
                        default:
                            return (T) Convert.ChangeType(false, typeof(bool));
                    }
                case TypeCode.Int16:
                    if (arg == null)
                        return (T) Convert.ChangeType(0, typeof(short));
                    return (T) Convert.ChangeType(Convert.ToInt16(arg, 16), typeof(short));
                case TypeCode.Int32:
                    if (arg == null)
                        return (T) Convert.ChangeType(0, typeof(int));
                    return (T) Convert.ChangeType(Convert.ToInt32(arg, 16), typeof(int));
                case TypeCode.Int64:
                    if (arg == null)
                        return (T) Convert.ChangeType(0, typeof(long));
                    return (T) Convert.ChangeType(Convert.ToInt64(arg, 16), typeof(long));
                case TypeCode.String:
                    var stringValue = Encoding.UTF8.GetString(Convert.FromHexString(arg));
                    return (T) Convert.ChangeType(stringValue, typeof(string));
                case TypeCode.UInt16:
                    if (arg == null)
                        return (T) Convert.ChangeType(0, typeof(ushort));
                    return (T) Convert.ChangeType(Convert.ToUInt16(arg, 16), typeof(ushort));
                case TypeCode.UInt32:
                    if (arg == null)
                        return (T) Convert.ChangeType(0, typeof(uint));
                    return (T) Convert.ChangeType(Convert.ToUInt32(arg, 16), typeof(uint));
                case TypeCode.UInt64:
                    if (arg == null)
                        return (T) Convert.ChangeType(0, typeof(ulong));
                    return (T) Convert.ChangeType(Convert.ToUInt64(arg, 16), typeof(ulong));
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

        public static Argument CreateArgumentFromInt16(short value, bool isOptional = false)
        {
            return isOptional
                ? CreateOptionalNumberArgument(4, value)
                : CreateArgumentFromBigInteger(new BigInteger(value));
        }

        public static Argument CreateArgumentFromUInt16(ushort value, bool isOptional = false)
        {
            return isOptional
                ? CreateOptionalNumberArgument(4, value, true)
                : CreateArgumentFromBigInteger(new BigInteger(value), true);
        }

        public static Argument CreateArgumentFromUInt32(uint value, bool isOptional = false)
        {
            return isOptional
                ? CreateOptionalNumberArgument(8, value, true)
                : CreateArgumentFromBigInteger(new BigInteger(value), true);
        }

        public static Argument CreateArgumentFromInt32(int value, bool isOptional = false)
        {
            return isOptional
                ? CreateOptionalNumberArgument(8, value)
                : CreateArgumentFromBigInteger(new BigInteger(value));
        }

        public static Argument CreateArgumentFromUInt64(ulong value, bool isOptional = false)
        {
            return isOptional
                ? CreateOptionalNumberArgument(16, value, true)
                : CreateArgumentFromBigInteger(new BigInteger(value), true);
        }

        public static Argument CreateArgumentFromInt64(long value, bool isOptional = false)
        {
            return isOptional
                ? CreateOptionalNumberArgument(16, value)
                : CreateArgumentFromBigInteger(new BigInteger(value));
        }

        public static Argument CreateArgumentFromAddress(Address address)
        {
            return CreateArgumentFromHex(address.Hex);
        }

        public static Argument CreateArgumentFromBigInteger(BigInteger bigInteger, bool isUnsigned = false)
        {
            if (bigInteger.IsZero)
                return new Argument(string.Empty);

            var bytes = bigInteger.ToByteArray(isUnsigned, true);
            return CreateArgumentFromBytes(bytes);
        }

        public static Argument CreateArgumentFromBalance(Balance balance)
        {
            return CreateArgumentFromBigInteger(balance.Value);
        }

        private static Argument CreateOptionalNumberArgument(int size, BigInteger number, bool isUnsigned = false)
        {
            var bytes = number.ToByteArray(isUnsigned: isUnsigned, isBigEndian: true);
            var hex = Convert.ToHexString(bytes);
            return new Argument("01" + hex.PadLeft(size, '0'));
        }
    }
}