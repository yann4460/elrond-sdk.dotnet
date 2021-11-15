using System;

namespace Erdcsharp.Domain.Exceptions
{
    public class WrongBinaryValueCodecException : Exception
    {
        public WrongBinaryValueCodecException()
            : base("Wrong binary argument")
        {
        }
    }
}
