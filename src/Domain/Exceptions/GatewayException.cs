using System;

namespace Elrond.Dotnet.Sdk.Domain.Exceptions
{
    public class GatewayException : Exception
    {
        public GatewayException(string errorMessage, string code)
            : base($"Error when calling Gateway : {errorMessage} with code : {code}")
        {
        }
    }
}