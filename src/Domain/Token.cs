using System;
using System.Numerics;
using System.Text.RegularExpressions;

namespace Erdcsharp.Domain
{
    public class Token
    {
        private readonly Regex  _nameValidation   = new Regex("^[a-zA-Z0-9]{3,20}$");
        private readonly Regex  _tickerValidation = new Regex("^[A-Z0-9]{3,10}$");
        public           string Name             { get; }
        public           string Ticker           { get; }
        public           int    DecimalPrecision { get; }

        public Token(string name, string ticker, int decimalPrecision)
        {
            if (!_nameValidation.IsMatch(name))
                throw new ArgumentException(
                                            "Length should be between 3 and 20 characters, alphanumeric characters only", nameof(name));

            if (!_tickerValidation.IsMatch(ticker))
                throw new ArgumentException(
                                            "Length should be between 3 and 10 characters, alphanumeric UPPERCASE characters only",
                                            nameof(ticker));

            if (decimalPrecision < 0 || decimalPrecision > 18)
                throw new ArgumentException("Should be between 0 and 18", nameof(decimalPrecision));

            Name             = name;
            Ticker           = ticker;
            DecimalPrecision = decimalPrecision;
        }

        /// <summary>
        /// Elrond eGold token (EGLD)
        /// </summary>
        /// <returns></returns>
        public static Token EGLD()
        {
            return new Token("ElrondeGold", Constants.EGLD, 18);
        }

        /// <summary>
        /// Create an ESDT (Elrond Standard Digital Token)
        /// </summary>
        /// <param name="name">The name of the token</param>
        /// <param name="ticker">The token ticker</param>
        /// <param name="decimalPrecision">Decimal precision of the token (max 18)</param>
        /// <returns></returns>
        public static Token ESDT(string name, string ticker, int decimalPrecision)
        {
            return new Token(name, ticker, decimalPrecision);
        }

        /// <summary>
        /// Create an ESDT (Elrond Standard Digital Token) NFT
        /// </summary>
        /// <param name="name">The name of the token</param>
        /// <param name="ticker">The token ticker</param>
        /// <returns></returns>
        public static Token ESDT_NFT(string name, string ticker)
        {
            return new Token(name, ticker, 0);
        }

        /// <summary>
        /// The value One
        /// </summary>
        /// <returns></returns>
        public BigInteger One()
        {
            var value = "1".PadRight(DecimalPrecision + 1, '0');
            return BigInteger.Parse(value);
        }

        /// <summary>
        /// The value Zero
        /// </summary>
        /// <returns></returns>
        public BigInteger Zero()
        {
            return new BigInteger(0);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
