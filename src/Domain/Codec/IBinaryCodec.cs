using System;

namespace Elrond.Dotnet.Sdk.Domain.Codec
{
    interface IBinaryCodec<T> where T : PrimitiveValue
    {
        Tuple<T, int> DecodeNested(byte[] data);
        T DecodeTopLevel(byte[] data);

        byte[] EncodeNested(T value);
        byte[] EncodeTopLevel(T value);
    }
}