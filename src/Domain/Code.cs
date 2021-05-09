using System;

namespace Elrond.Dotnet.Sdk.Domain
{
    public class Code
    {
        public string Value { get; }

        public Code(byte[] bytes)
        {
            Value = Convert.ToHexString(bytes);
        }
    }
}