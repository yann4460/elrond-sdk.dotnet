using System;

namespace Elrond.Dotnet.Sdk.Domain.Exceptions
{
    public class WrongBinaryValueCodecException : Exception
    {
        public WrongBinaryValueCodecException()
            : base("Wrong binary argument")
        {
        }
    }
}