using System;

namespace Elrond.Dotnet.Sdk.Domain.Values
{
    public class TypeValue
    {
        public string BinaryType { get; }
        public string RustType { get; }
        private readonly int? _sizeInBytes;
        private readonly bool? _withSign;
        private readonly FieldDefinition[] _fieldDefinitions;

        public TypeValue(string binaryType, string rustType, int? sizeInBytes = null, bool? withSign = null)
        {
            BinaryType = binaryType;
            RustType = rustType;
            _sizeInBytes = sizeInBytes;
            _withSign = withSign;
        }

        public TypeValue(string binaryType, string rustType, FieldDefinition[] fieldDefinitions)
        {
            BinaryType = binaryType;
            RustType = rustType;
            _fieldDefinitions = fieldDefinitions;
        }


        public int SizeInBytes()
        {
            return _sizeInBytes ?? U32TypeValue.SizeInBytes();
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

        public static class BinaryTypes
        {
            public const string Boolean = nameof(Boolean);
            public const string Address = nameof(Address);
            public const string Numeric = nameof(Numeric);
            public const string Struct = nameof(Struct);
            public const string Bytes = nameof(Bytes);
        }

        public static class RustTypes
        {
            public const string u8 = "u8";
            public const string u16 = "u16";
            public const string u32 = "u32";
            public const string u64 = "u64";
            public const string BigUint = "BigUint";

            public const string i8 = "i8";
            public const string i16 = "i16";
            public const string i32 = "i32";
            public const string i64 = "i64";
            public const string Bigint = "Bigint";

            public const string Bool = "bool";
            public const string Bytes = "bytes";
            public const string Address = "Address";
            public const string H256 = "H256";
            public const string TokenIdentifier = "TokenIdentifier";
        }

        public static TypeValue U8TypeValue => new TypeValue(BinaryTypes.Numeric, RustTypes.u8, 1, false);
        public static TypeValue I8TypeValue => new TypeValue(BinaryTypes.Numeric, RustTypes.i8, 1, true);

        public static TypeValue U16TypeValue => new TypeValue(BinaryTypes.Numeric, RustTypes.u16, 2, false);
        public static TypeValue I16TypeValue => new TypeValue(BinaryTypes.Numeric, RustTypes.i16, 2, true);

        public static TypeValue U32TypeValue => new TypeValue(BinaryTypes.Numeric, RustTypes.u32, 4, false);
        public static TypeValue I32TypeValue => new TypeValue(BinaryTypes.Numeric, RustTypes.i32, 4, true);

        public static TypeValue U64TypeValue => new TypeValue(BinaryTypes.Numeric, RustTypes.u64, 8, false);
        public static TypeValue I64TypeValue => new TypeValue(BinaryTypes.Numeric, RustTypes.i64, 8, true);

        public static TypeValue BigUintTypeValue => new TypeValue(BinaryTypes.Numeric, RustTypes.BigUint, null, false);
        public static TypeValue BigIntTypeValue => new TypeValue(BinaryTypes.Numeric, RustTypes.Bigint, null, true);

        public static TypeValue BooleanValue => new TypeValue(BinaryTypes.Boolean, RustTypes.Bool);
        public static TypeValue AddressValue => new TypeValue(BinaryTypes.Address, RustTypes.Address);
        public static TypeValue TokenIdentifierValue => new TypeValue(BinaryTypes.Bytes, RustTypes.TokenIdentifier);
        public static TypeValue BytesValue => new TypeValue(BinaryTypes.Bytes, RustTypes.Bytes);
        public static TypeValue H256Value => new TypeValue(BinaryTypes.Bytes, RustTypes.H256);

        public static TypeValue StructValue(string name, FieldDefinition[] fieldDefinitions) =>
            new TypeValue(BinaryTypes.Struct, name, fieldDefinitions);


        public static TypeValue FromRustType(string rustType)
        {
            switch (rustType)
            {
                case RustTypes.u8:
                    return U8TypeValue;
                case RustTypes.u16:
                    return U16TypeValue;
                case RustTypes.u32:
                    return U32TypeValue;
                case RustTypes.u64:
                    return U64TypeValue;
                case RustTypes.BigUint:
                    return BigUintTypeValue;

                case RustTypes.i8:
                    return I8TypeValue;
                case RustTypes.i16:
                    return I16TypeValue;
                case RustTypes.i32:
                    return I32TypeValue;
                case RustTypes.i64:
                    return I64TypeValue;
                case RustTypes.Bigint:
                    return BigIntTypeValue;

                case RustTypes.Bytes:
                    return BytesValue;
                case RustTypes.Bool:
                    return BooleanValue;
                case RustTypes.Address:
                    return AddressValue;
                case RustTypes.TokenIdentifier:
                    return TokenIdentifierValue;
                default:
                    throw new ArgumentOutOfRangeException(nameof(rustType));
            }
        }

        public FieldDefinition[] GetFieldDefinitions()
        {
            return _fieldDefinitions;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_sizeInBytes, _withSign, RustType);
        }
    }
}