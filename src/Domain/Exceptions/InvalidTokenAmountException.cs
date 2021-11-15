using System;

namespace Erdcsharp.Domain.Exceptions
{
    public class InvalidTokenAmountException : Exception
    {
        public InvalidTokenAmountException(string value)
            : base($"Invalid TokenAmount {value}")
        {
        }
    }
}
