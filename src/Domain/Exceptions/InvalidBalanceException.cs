using System;

namespace Elrond.Dotnet.Sdk.Domain.Exceptions
{
    public class InvalidBalanceException : Exception
    {
        public InvalidBalanceException(string value)
            : base($"Invalid balance {value}")
        {
        }
    }
}