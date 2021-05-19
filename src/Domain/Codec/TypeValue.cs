﻿namespace Elrond.Dotnet.Sdk.Domain.Codec
{
    public class TypeValue
    {
        private readonly string _name;
        private readonly int? _sizeInBytes;
        private readonly bool? _withSign;

        public TypeValue(string name, int? sizeInBytes, bool? withSign)
        {
            _name = name;
            _sizeInBytes = sizeInBytes;
            _withSign = withSign;
        }

        public int SizeInBytes()
        {
            if (_sizeInBytes.HasValue)
            {
                return _sizeInBytes.Value;
            }

            return U32Type().SizeInBytes(); // Arbitrary size
        }

        public bool HasSign()
        {
            return _withSign ?? false;
        }

        public bool HasFixedSize()
        {
            return _sizeInBytes.HasValue;
        }

        public bool HasArbitrarySize()
        {
            return !HasFixedSize();
        }

        public static TypeValue U8Type() => new TypeValue("u8", 1, false);
        public static TypeValue I8Type() => new TypeValue("i8", 1, true);

        public static TypeValue U16Type() => new TypeValue("u16", 2, false);
        public static TypeValue I16Type() => new TypeValue("i16", 2, true);

        public static TypeValue U32Type() => new TypeValue("u32", 4, false);
        public static TypeValue I32Type() => new TypeValue("i32", 4, true);

        public static TypeValue U64Type() => new TypeValue("u64", 8, false);
        public static TypeValue I64Type() => new TypeValue("i64", 8, true);

        public static TypeValue BigUintType() => new TypeValue("BigUint", null, false);
        public static TypeValue BigIntType() => new TypeValue("Bigint", null, true);
    }
}