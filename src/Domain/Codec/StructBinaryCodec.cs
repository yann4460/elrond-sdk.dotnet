using System;
using System.Collections.Generic;

namespace Elrond.Dotnet.Sdk.Domain.Codec
{
    public class StructBinaryCodec : IBinaryCodec
    {
        public IEnumerable<TypeValue> Types => new[] {TypeValue.Struct};

        public StructBinaryCodec()
        {
        }

        public (IBinaryType Value, int BytesLength) DecodeNested(byte[] data, TypeValue type)
        {
            throw new NotImplementedException();
        }

        public IBinaryType DecodeTopLevel(byte[] data, TypeValue type)
        {
            throw new NotImplementedException();
        }

        public byte[] EncodeNested(IBinaryType value)
        {
            throw new NotImplementedException();
        }

        public byte[] EncodeTopLevel(IBinaryType value)
        {
            throw new NotImplementedException();
        }
    }
}