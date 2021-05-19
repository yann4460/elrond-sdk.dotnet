namespace Elrond.Dotnet.Sdk.Domain.Codec
{
    interface IBinaryCodec<T> where T : PrimitiveValue
    {
        (T Value, int BytesLength) DecodeNested(byte[] data, TypeValue type = null);
        T DecodeTopLevel(byte[] data, TypeValue type = null);

        byte[] EncodeNested(T value);
        byte[] EncodeTopLevel(T value);
    }
}