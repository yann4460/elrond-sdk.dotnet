using Erdcsharp.Domain.Values;

namespace Erdcsharp.Domain.Codec
{
    internal interface IBinaryCodec
    {
        /// <summary>
        /// <see cref="TypeValue.BinaryTypes"/> constants
        /// </summary>
        string Type { get; }

        (IBinaryType Value, int BytesLength) DecodeNested(byte[] data, TypeValue type);

        IBinaryType DecodeTopLevel(byte[] data, TypeValue type);

        byte[] EncodeNested(IBinaryType value);

        byte[] EncodeTopLevel(IBinaryType value);
    }
}
