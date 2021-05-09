using System;
using System.Threading.Tasks;
using Sdk.Provider;
using Sdk.Provider.Dtos;

namespace Sdk.Domain
{
    public class GasLimit
    {
        public long Value { get; }

        public GasLimit(long value)
        {
            Value = value;
        }

        public static GasLimit ForTransfer(ConstantsDto constants, TransactionRequestDto transaction)
        {
            var value = constants.MinGasLimit;
            if (string.IsNullOrEmpty(transaction.Data)) return new GasLimit(value);
            var bytes = Convert.FromBase64String(transaction.Data);
            value += constants.GasPerDataByte * bytes.Length;

            return new GasLimit(value);
        }

        public static async Task<GasLimit> ForTransaction(TransactionRequest transactionRequest, IElrondProvider provider)
        {
            var cost = await provider.GetTransactionCost(transactionRequest.GetTransactionRequest());
            if (cost.Data.TxGasUnits == 0)
                throw new Exception($"Unable to get cost of transaction : {cost.Data.ReturnMessage}");
            
            return new GasLimit(cost.Data.TxGasUnits);
        }
    }
}