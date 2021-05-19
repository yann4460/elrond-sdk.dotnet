using System;

namespace Elrond.Dotnet.Sdk.Domain.Exceptions
{
    public class BinaryCodecException : Exception
    {
        public BinaryCodecException(string message)
            : base(message)
        {
        }
    }
}