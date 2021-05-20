using System.Collections.Generic;

namespace Elrond.Dotnet.Sdk.Domain.Codec
{
    internal interface IBinaryCodec
    {
        IEnumerable<TypeValue> Types { get; }

        (IBinaryType Value, int BytesLength) DecodeNested(byte[] data, TypeValue type);

        IBinaryType DecodeTopLevel(byte[] data, TypeValue type);

        byte[] EncodeNested(IBinaryType value);

        byte[] EncodeTopLevel(IBinaryType value);
    }
}