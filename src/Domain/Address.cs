using System;
using Erdcsharp.Cryptography;
using Erdcsharp.Domain.Exceptions;
using Erdcsharp.Domain.Helper;
using Erdcsharp.Domain.Values;

namespace Erdcsharp.Domain
{
    public class Address : BaseBinaryValue
    {
        // The human-readable-part of the bech32 addresses.
        private const string Hrp                          = Constants.Hrp;
        private const int    PubKeyLength                 = 32;
        private const string SmartContractHexPubKeyPrefix = "0000000000000000";

        private Address(string hex, string bech32)
            : base(TypeValue.AddressValue)
        {
            Bech32 = bech32.ToLowerInvariant();
            Hex    = hex.ToUpperInvariant();
        }

        /// <summary>
        /// Returns the bech32 representation of the address
        /// </summary>
        public string Bech32 { get; }

        /// <summary>
        ///  Returns the hex representation of the address (pubkey)
        /// </summary>
        public string Hex { get; }

        /// <summary>
        /// Creates an address object from a Buffer
        /// </summary>
        /// <param name="data">The buffer</param>
        /// <returns>Address</returns>
        public static Address FromBytes(byte[] data)
        {
            var hex    = Converter.ToHexString(data);
            var bech32 = Bech32Engine.Encode(Hrp, data);
            return new Address(hex, bech32);
        }

        /// <summary>
        /// Creates an address from a string (Hex or bech32)
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Address</returns>
        public static Address From(string value)
        {
            try
            {
                return IsValidHex(value) ? FromHex(value) : FromBech32(value);
            }
            catch
            {
                throw new CannotCreateAddressException(value);
            }
        }

        /// <summary>
        /// Creates an address from a hex string
        /// </summary>
        /// <param name="hex"></param>
        /// <returns>Address</returns>
        public static Address FromHex(string hex)
        {
            try
            {
                var bech32 = Bech32Engine.Encode(Hrp, Converter.FromHexString(hex));
                return new Address(hex, bech32);
            }
            catch
            {
                throw new CannotCreateAddressException(hex);
            }
        }

        /// <summary>
        /// Creates an address from a bech32 string
        /// </summary>
        /// <param name="bech32"></param>
        /// <returns>Address</returns>
        public static Address FromBech32(string bech32)
        {
            try
            {
                Bech32Engine.Decode(bech32, out _, out var data);
                var hex = Converter.ToHexString(data);
                return new Address(hex, bech32);
            }
            catch
            {
                throw new CannotCreateAddressException(bech32);
            }
        }

        /// <summary>
        /// Creates the Zero address (the one that should be used when deploying smart contracts)
        /// </summary>
        /// <returns>Address</returns>
        public static Address Zero()
        {
            const string hex = "0000000000000000000000000000000000000000000000000000000000000000";
            return FromHex(hex);
        }

        public byte[] PublicKey()
        {
            return Converter.FromHexString(Hex);
        }

        public bool IsContractAddress() => Hex.StartsWith(SmartContractHexPubKeyPrefix);

        public override string ToString()
        {
            return Bech32;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Address item))
            {
                return false;
            }

            return Hex.Equals(item.Hex, StringComparison.CurrentCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            return Hex.GetHashCode();
        }

        private static bool IsValidHex(string value)
        {
            return value.FromHex().Length == PubKeyLength;
        }
    }
}
