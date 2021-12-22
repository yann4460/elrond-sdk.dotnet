using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Erdcsharp.Domain.Codec;
using Erdcsharp.Domain.Values;

namespace Elrond.Dotnet.Sdk.Domain.Codec
{
    public class EnumBinaryCodec : IBinaryCodec
    {
        private readonly BinaryCodec _binaryCodec;
        public           string      Type => TypeValue.BinaryTypes.Enum;

        public EnumBinaryCodec(BinaryCodec binaryCodec)
        {
            _binaryCodec = binaryCodec;
        }

        public (IBinaryType Value, int BytesLength) DecodeNested(byte[] data, TypeValue type)
        {
            var (value, bytesLength) = this._binaryCodec.DecodeNested(data, TypeValue.U8TypeValue);
            var enumValue = EnumValue.FromDiscriminant(type, (int)value.ValueOf<NumericValue>().Number);
            return (enumValue, bytesLength);
        }

        public IBinaryType DecodeTopLevel(byte[] data, TypeValue type)
        {
            var value        = this._binaryCodec.DecodeTopLevel(data, TypeValue.U8TypeValue);
            var discriminant = (int)value.ValueOf<NumericValue>().Number;
            var enumValue = EnumValue.FromDiscriminant(type, discriminant);
            return enumValue;
        }

        public byte[] EncodeNested(IBinaryType value)
        {
            var enumValue = value.ValueOf<EnumValue>();
            var numValue  = new NumericValue(TypeValue.U8TypeValue, enumValue.Variant.Discriminant);
            var buffer    = this._binaryCodec.EncodeNested(numValue);
            return buffer;
        }

        public byte[] EncodeTopLevel(IBinaryType value)
        {
            var enumValue = value.ValueOf<EnumValue>();
            var numValue = new NumericValue(TypeValue.U8TypeValue, enumValue.Variant.Discriminant);
            var buffer = this._binaryCodec.EncodeTopLevel(numValue);
            return buffer;
        }
    }
}
