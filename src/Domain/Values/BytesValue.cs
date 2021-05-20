﻿namespace Elrond.Dotnet.Sdk.Domain.Values
{
    public class BytesValue : IBinaryType
    {
        public BytesValue(byte[] data, TypeValue type)
        {
            Buffer = data;
            Type = type;
        }

        public int GetLength()
        {
            return Buffer.Length;
        }

        public TypeValue Type { get; }

        public byte[] Buffer { get; }
    }
}