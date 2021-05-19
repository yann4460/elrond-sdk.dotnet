using System;

namespace Elrond.Dotnet.Sdk.Domain.Codec
{
    interface IBinaryCodec<T> where T : PrimitiveValue
    {
        (T Value, int BytesLength) DecodeNested(byte[] data);
        T DecodeTopLevel(byte[] data);

        byte[] EncodeNested(T value);
        byte[] EncodeTopLevel(T value);
    }
}