using System.Globalization;
using System.Numerics;
using Elrond.Dotnet.Sdk.Domain.Exceptions;

namespace Elrond.Dotnet.Sdk.Domain.Values
{
    public class BalanceValue : NumericValue
    {
        const long OneEgld = 1000000000000000000;
        private const int Denomination = 18;

        public BalanceValue(long value)
            : base(TypeValue.BigUintTypeValue, new BigInteger(value))
        {
        }

        public BalanceValue(string value)
            : base(TypeValue.BigUintTypeValue, BigInteger.Parse(value))
        {
            if (Number.Sign == -1)
                throw new InvalidBalanceException(value);
        }

        /// <summary>
        /// Returns the string representation of the value (as eGLD currency).
        /// </summary>
        /// <returns></returns>
        public string ToCurrencyString()
        {
            var denominated = ToDenominated();
            return $"{denominated} EGLD";
        }

        public string ToDenominated()
        {
            var padded = Number.ToString().PadLeft(Denomination, '0');

            var start = (padded.Length - Denomination);
            start = start < 0 ? 0 : start;

            var decimals = padded.Substring(start, Denomination);
            var integer = start == 0 ? "0" : padded.Substring(0, start);

            return $"{integer}.{decimals}";
        }

        /// <summary>
        /// Creates a balance object from an eGLD value (denomination will be applied).
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static BalanceValue EGLD(string value)
        {
            var decimalValue = decimal.Parse(value, CultureInfo.InvariantCulture);
            var p = decimalValue * OneEgld;
            var bigGold = new BigInteger(p);

            return new BalanceValue(bigGold.ToString());
        }

        public static BalanceValue Zero()
        {
            return new BalanceValue(0);
        }

        public override string ToString()
        {
            return Number.ToString();
        }
    }
}