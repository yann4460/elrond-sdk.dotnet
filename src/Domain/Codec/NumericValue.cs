using System.Numerics;
using Elrond.Dotnet.Sdk.Domain.Exceptions;

namespace Elrond.Dotnet.Sdk.Domain.Codec
{
    public class NumericValue : PrimitiveValue
    {
        private readonly BigInteger _number;

        public NumericValue(TypeValue type, BigInteger number)
        {
            Type = type;
            _number = number;

            if (number.Sign == -1 && !type.HasSign())
                throw new BinaryCodecException("negative, but type is unsigned");
        }

        public TypeValue Type { get; }

        public BigInteger ValueOf()
        {
            return _number;
        }

        public static NumericValue U8Value(byte value) => new NumericValue(TypeValue.U8Type(), new BigInteger(value));
        public static NumericValue I8Value(sbyte value) => new NumericValue(TypeValue.I8Type(), new BigInteger(value));

        public static NumericValue U16Value(ushort value) => new NumericValue(TypeValue.U16Type(), new BigInteger(value));
        public static NumericValue I16Value(short value) => new NumericValue(TypeValue.I16Type(), new BigInteger(value));

        public static NumericValue U32Value(uint value) => new NumericValue(TypeValue.U32Type(), new BigInteger(value));
        public static NumericValue I32Value(int value) => new NumericValue(TypeValue.I32Type(), new BigInteger(value));

        public static NumericValue U64Value(ulong value) => new NumericValue(TypeValue.U8Type(), new BigInteger(value));
        public static NumericValue I64Value(long value) => new NumericValue(TypeValue.I8Type(), new BigInteger(value));

        public static NumericValue BigUintValue(BigInteger value) => new NumericValue(TypeValue.BigUintType(), value);
        public static NumericValue BigIntValue(BigInteger value) => new NumericValue(TypeValue.BigIntType(), value);
    }
}