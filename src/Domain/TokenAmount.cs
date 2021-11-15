using System.Linq;
using System.Numerics;
using Erdcsharp.Domain.Exceptions;

namespace Erdcsharp.Domain
{
    public class TokenAmount
    {
        public Token      Token  { get; }
        public BigInteger Value { get; }

        private TokenAmount(long value, Token token)
        {
            Token  = token;
            Value = new BigInteger(value);
        }

        private TokenAmount(string value, Token token)
        {
            Token  = token;
            Value = BigInteger.Parse(value);
            if (Value.Sign == -1)
                throw new InvalidTokenAmountException(value);
        }

        /// <summary>
        /// Returns the string representation of the value as Token currency.
        /// </summary>
        /// <returns></returns>
        public string ToCurrencyString()
        {
            var denominated = ToDenominated();
            return $"{denominated} {Token.Ticker}";
        }

        /// <summary>
        /// String representation of the denominated value
        /// </summary>
        /// <returns></returns>
        public string ToDenominated()
        {
            var padded = Value.ToString().PadLeft(Token.DecimalPrecision, '0');

            var start = (padded.Length - Token.DecimalPrecision);
            start = start < 0 ? 0 : start;

            var decimals = padded.Substring(start, Token.DecimalPrecision);
            var integer  = start == 0 ? "0" : padded.Substring(0, start);

            return $"{integer}.{decimals}";
        }

        /// <summary>
        /// Creates a token amount object from an eGLD value (denomination will be applied).
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public static TokenAmount EGLD(string value)
        {
            var egld        = Token.EGLD();
            var split       = value.Split('.');
            var integerPart = split.FirstOrDefault() ?? "0";
            var decimalPart = split.Length == 2 ? split[1] : string.Empty;
            var full        = $"{integerPart}{decimalPart.PadRight(egld.DecimalPrecision, '0')}";
            return new TokenAmount(full, Token.EGLD());
        }

        /// <summary>
        /// Create a token amount object from a value (denomination will be applied)
        /// </summary>
        /// <param name="value">Amount</param>
        /// <param name="token">Token, default is EGLD</param>
        /// <returns></returns>
        public static TokenAmount ESDT(string value, Token token)
        {
            var split       = value.Split('.');
            var integerPart = split.FirstOrDefault() ?? "0";
            var decimalPart = split.Length == 2 ? split[1] : string.Empty;
            var full        = $"{integerPart}{decimalPart.PadRight(token.DecimalPrecision, '0')}";
            return new TokenAmount(full, token);
        }

        public static TokenAmount From(string value, Token token = null)
        {
            if (token == null)
                token = Token.EGLD();
            return new TokenAmount(value, token);
        }

        /// <summary>
        /// Value zero
        /// </summary>
        /// <param name="token">Token, default is EGLD</param>
        /// <returns></returns>
        public static TokenAmount Zero(Token token = null)
        {
            if (token == null)
                token = Token.EGLD();

            return new TokenAmount(0, token);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
