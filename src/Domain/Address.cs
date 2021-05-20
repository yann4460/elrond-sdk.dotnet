using System;
using Elrond.Dotnet.Sdk.Cryptography;
using Elrond.Dotnet.Sdk.Domain.Codec;
using Elrond.Dotnet.Sdk.Domain.Values;

namespace Elrond.Dotnet.Sdk.Domain
{
    public class Address : IBinaryType
    {
        public string Bech32 { get; }
        public string Hex { get; }
        private const string Hrp = "erd";

        private Address(string hex, string bech32)
        {
            Bech32 = bech32.ToLowerInvariant();
            Hex = hex.ToLowerInvariant();
        }

        public static Address FromBytes(byte[] data)
        {
            var hex = Convert.ToHexString(data);
            var bech32 = Bech32Engine.Encode(Hrp, data);
            return new Address(hex, bech32);
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

        public byte[] PublicKey()
        {
            return Convert.FromHexString(Hex);
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

        public TypeValue Type => TypeValue.Address;

        public IBinaryType ValueOf()
        {
            return this;
        }

        public T ValueOf<T>() where T : IBinaryType
        {
            throw new NotImplementedException();
        }
    }
}