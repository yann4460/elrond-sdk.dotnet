using System;

namespace Sdk.Domain
{
    public class CodeMetadata
    {
        private readonly bool _upgradeable;
        private readonly bool _readable;
        private readonly bool _payable;
        public string Value { get; }

        public CodeMetadata(bool upgradeable, bool readable, bool payable)
        {
            _upgradeable = upgradeable;
            _readable = readable;
            _payable = payable;
            Value = Convert.ToHexString(ToByte());
        }

        // Converts the metadata to the protocol-friendly representation.
        private byte[] ToByte()
        {
            ByteZero byteZero = 0;
            ByteOne byteOne = 0;

            if (_upgradeable)
            {
                byteZero |= ByteZero.Upgradeable;
            }

            if (_readable)
            {
                byteZero |= ByteZero.Readable;
            }

            if (_payable)
            {
                byteOne |= ByteOne.Payable;
            }

            var bytes = new[] {(byte) byteZero, (byte) byteOne};

            return bytes;
        }


        [Flags]
        enum ByteZero : byte
        {
            Upgradeable = 1,
            Reserved2 = 2,
            Readable = 4
        }

        [Flags]
        enum ByteOne : byte
        {
            Reserved1 = 1,
            Payable = 2
        }
    }
}