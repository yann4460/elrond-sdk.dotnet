using System;
using System.Threading.Tasks;
using Elrond.Dotnet.Sdk.Provider;

namespace Elrond.Dotnet.Sdk.Domain
{
    public class GasLimit
    {
        public long Value { get; }

        public GasLimit(long value)
        {
            Value = value;
        }

        public static GasLimit ForTransfer(Constants constants, TransactionRequest transaction)
        {
            var value = constants.MinGasLimit;
            if (string.IsNullOrEmpty(transaction.Data)) return new GasLimit(value);
            var bytes = Convert.FromBase64String(transaction.Data);
            value += constants.GasPerDataByte * bytes.Length;

            return new GasLimit(value);
        }

        public static GasLimit ForSmartContractCall(Constants constants, TransactionRequest transaction)
        {
            var value = constants.MinGasLimit + 6000000;
            if (string.IsNullOrEmpty(transaction.Data))
                return new GasLimit(value);
            
            var bytes = Convert.FromBase64String(transaction.Data);
            value += constants.GasPerDataByte * bytes.Length;

            return new GasLimit(value);
        }

        public static async Task<GasLimit> ForTransaction(TransactionRequest transactionRequest,
            IElrondProvider provider)
        {
            var cost = await provider.GetTransactionCost(transactionRequest.GetTransactionRequest());
            if (cost.Data.TxGasUnits == 0)
                throw new Exception($"Unable to get cost of transaction : {cost.Data.ReturnMessage}");

            return new GasLimit(cost.Data.TxGasUnits);
        }
    }
}