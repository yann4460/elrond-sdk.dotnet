using System;

namespace Erdcsharp.Domain.Exceptions
{
    public class CannotCreateAddressException : Exception
    {
        public CannotCreateAddressException(string input)
            : base($"Cannot create address from {input}")
        {
        }
    }
}
