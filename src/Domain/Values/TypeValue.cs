using System.Linq;

namespace Erdcsharp.Domain.Values
{
    public class TypeValue
    {
        public string Name { get; private set; }

        public string      BinaryType { get; }
        public string      RustType   { get; }
        public TypeValue   InnerType  { get; }
        public TypeValue[] MultiTypes { get; }

        private readonly int?                    _sizeInBytes;
        private readonly bool?                   _withSign;
        private readonly FieldDefinition[]       _fieldDefinitions;
        private readonly EnumVariantDefinition[] _variantDefinitions;

        public TypeValue(string binaryType, string rustType, int? sizeInBytes = null, bool? withSign = null)
        {
            BinaryType   = binaryType;
            RustType     = rustType;
            _sizeInBytes = sizeInBytes;
            _withSign    = withSign;
        }

        public TypeValue(string binaryType, string rustType, FieldDefinition[] fieldDefinitions)
        {
            BinaryType        = binaryType;
            RustType          = rustType;
            _fieldDefinitions = fieldDefinitions;
        }

        public TypeValue(string binaryType, string rustType, EnumVariantDefinition[] variantDefinitions)
        {
            BinaryType          = binaryType;
            RustType            = rustType;
            _variantDefinitions = variantDefinitions;
        }

        public TypeValue(string binaryType, TypeValue innerType = null)
        {
            BinaryType = binaryType;
            InnerType  = innerType;
        }

        public TypeValue(string binaryType, TypeValue[] multiTypes)
        {
            BinaryType = binaryType;
            MultiTypes = multiTypes;
        }

        public void SetName(string name) => Name = name;

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
            public const string Boolean         = nameof(Boolean);
            public const string Address         = nameof(Address);
            public const string Numeric         = nameof(Numeric);
            public const string Struct          = nameof(Struct);
            public const string Enum            = nameof(Enum);
            public const string Bytes           = nameof(Bytes);
            public const string TokenIdentifier = nameof(TokenIdentifier);
            public const string Option          = nameof(Option);
            public const string Multi           = nameof(Multi);
        }

        public static class RustTypes
        {
            public const string U8      = "u8";
            public const string U16     = "u16";
            public const string U32     = "u32";
            public const string U64     = "u64";
            public const string BigUint = "BigUint";

            public const string I8     = "i8";
            public const string I16    = "i16";
            public const string I32    = "i32";
            public const string I64    = "i64";
            public const string Bigint = "BigInt";

            public const string Bool            = "bool";
            public const string Bytes           = "bytes";
            public const string Address         = "Address";
            public const string H256            = "H256";
            public const string TokenIdentifier = "TokenIdentifier";
        }

        public static TypeValue U8TypeValue => new TypeValue(BinaryTypes.Numeric, RustTypes.U8, 1, false);
        public static TypeValue I8TypeValue => new TypeValue(BinaryTypes.Numeric, RustTypes.I8, 1, true);

        public static TypeValue U16TypeValue => new TypeValue(BinaryTypes.Numeric, RustTypes.U16, 2, false);
        public static TypeValue I16TypeValue => new TypeValue(BinaryTypes.Numeric, RustTypes.I16, 2, true);

        public static TypeValue U32TypeValue => new TypeValue(BinaryTypes.Numeric, RustTypes.U32, 4, false);
        public static TypeValue I32TypeValue => new TypeValue(BinaryTypes.Numeric, RustTypes.I32, 4, true);

        public static TypeValue U64TypeValue => new TypeValue(BinaryTypes.Numeric, RustTypes.U64, 8, false);
        public static TypeValue I64TypeValue => new TypeValue(BinaryTypes.Numeric, RustTypes.I64, 8, true);

        public static TypeValue BigUintTypeValue => new TypeValue(BinaryTypes.Numeric, RustTypes.BigUint, null, false);
        public static TypeValue BigIntTypeValue  => new TypeValue(BinaryTypes.Numeric, RustTypes.Bigint, null, true);

        public static TypeValue BooleanValue => new TypeValue(BinaryTypes.Boolean, RustTypes.Bool);
        public static TypeValue AddressValue => new TypeValue(BinaryTypes.Address, RustTypes.Address);

        public static TypeValue TokenIdentifierValue => new TypeValue(BinaryTypes.TokenIdentifier, RustTypes.TokenIdentifier);

        public static TypeValue BytesValue => new TypeValue(BinaryTypes.Bytes, RustTypes.Bytes);
        public static TypeValue H256Value  => new TypeValue(BinaryTypes.Bytes, RustTypes.H256);

        public static TypeValue EnumValue(string name, EnumVariantDefinition[] variantDefinitions) =>
            new TypeValue(BinaryTypes.Enum, name, variantDefinitions);

        public static TypeValue OptionValue(TypeValue innerType = null) => new TypeValue(BinaryTypes.Option, innerType);
        public static TypeValue MultiValue(TypeValue[] multiTypes) => new TypeValue(BinaryTypes.Multi, multiTypes);

        public static TypeValue StructValue(string name, FieldDefinition[] fieldDefinitions) =>
            new TypeValue(BinaryTypes.Struct, name, fieldDefinitions);

        public static TypeValue FromRustType(string rustType)
        {
            switch (rustType)
            {
                case RustTypes.U8:
                    return U8TypeValue;
                case RustTypes.U16:
                    return U16TypeValue;
                case RustTypes.U32:
                    return U32TypeValue;
                case RustTypes.U64:
                    return U64TypeValue;
                case RustTypes.BigUint:
                    return BigUintTypeValue;

                case RustTypes.I8:
                    return I8TypeValue;
                case RustTypes.I16:
                    return I16TypeValue;
                case RustTypes.I32:
                    return I32TypeValue;
                case RustTypes.I64:
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
                    return null;
            }
        }

        public FieldDefinition[] GetFieldDefinitions()
        {
            return _fieldDefinitions;
        }

        public EnumVariantDefinition GetVariantByDiscriminant(int discriminant)
        {
            return _variantDefinitions.SingleOrDefault(s => s.Discriminant == discriminant);
        }
        public EnumVariantDefinition GetVariantByName(string name)
        {
            return _variantDefinitions.SingleOrDefault(s => s.Name == name);
        }
    }
}
