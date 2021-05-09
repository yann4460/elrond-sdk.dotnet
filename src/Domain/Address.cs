using System;
using Elrond.Dotnet.Sdk.Cryptography;

namespace Elrond.Dotnet.Sdk.Domain
{
    public class Address
    {
        public string Bech32 { get; }
        public string Hex { get; }
        private const string Hrp = "erd";

        private Address(string hex, string bech32)
        {
            Bech32 = bech32.ToLowerInvariant();
            Hex = hex.ToLowerInvariant();
        }

        public static Address FromHex(string hex)
        {
            var bech32 = Bech32Engine.Encode(Hrp, Convert.FromHexString(hex));
            return new Address(hex, bech32);
        }

        public static Address FromBech32(string bech32)
        {
            Bech32Engine.Decode(bech32, out _, out var data);
            var hex = Convert.ToHexString(data);
            return new Address(hex, bech32);
        }

        /// <summary>
        /// Smart contract deployment address
        /// </summary>
        /// <returns></returns>
        public static Address Zero()
        {
            var hex = "0000000000000000000000000000000000000000000000000000000000000000";
            return FromHex(hex);
        }

        public override string ToString()
        {
            return Bech32;
        }
    }
}