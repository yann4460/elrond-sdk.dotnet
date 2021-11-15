using System;

namespace Erdcsharp.Domain.Exceptions
{
    public class GatewayException : Exception
    {
        public GatewayException(string errorMessage, string code)
            : base($"Error when calling Gateway : {errorMessage} with smartContractCode : {code}")
        {
        }
    }
}
