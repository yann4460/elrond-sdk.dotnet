﻿using System.Numerics;
using Elrond.Dotnet.Sdk.Domain.Exceptions;

namespace Elrond.Dotnet.Sdk.Domain.Values
{
    public class NumericValue : IBinaryType
    {
        public byte[] Buffer { get; }

        public NumericValue(TypeValue type, BigInteger number)
        {
            Type = type;
            Number = number;
            Buffer = number.ToByteArray(!type.HasSign(), true);
            if (number.Sign == -1 && !type.HasSign())
                throw new BinaryCodecException("negative, but binaryType is unsigned");
        }

        public BigInteger Number { get; }

        public TypeValue Type { get; }


        public static NumericValue U8Value(byte value) =>
            new NumericValue(TypeValue.U8TypeValue, new BigInteger(value));

        public static NumericValue I8Value(sbyte value) =>
            new NumericValue(TypeValue.I8TypeValue, new BigInteger(value));

        public static NumericValue U16Value(ushort value) =>
            new NumericValue(TypeValue.U16TypeValue, new BigInteger(value));

        public static NumericValue I16Value(short value) =>
            new NumericValue(TypeValue.I16TypeValue, new BigInteger(value));

        public static NumericValue U32Value(uint value) =>
            new NumericValue(TypeValue.U32TypeValue, new BigInteger(value));

        public static NumericValue I32Value(int value) =>
            new NumericValue(TypeValue.I32TypeValue, new BigInteger(value));

        public static NumericValue U64Value(ulong value) =>
            new NumericValue(TypeValue.U64TypeValue, new BigInteger(value));

        public static NumericValue I64Value(long value) =>
            new NumericValue(TypeValue.I64TypeValue, new BigInteger(value));

        public static NumericValue BigUintValue(BigInteger value) =>
            new NumericValue(TypeValue.BigUintTypeValue, value);

        public static NumericValue BigIntValue(BigInteger value) => new NumericValue(TypeValue.BigIntTypeValue, value);
    }
}