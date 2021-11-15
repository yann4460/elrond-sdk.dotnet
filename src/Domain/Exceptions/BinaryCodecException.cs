using System;

namespace Erdcsharp.Domain.Exceptions
{
    public class BinaryCodecException : Exception
    {
        public BinaryCodecException(string message)
            : base(message)
        {
        }
    }
}
