using System;
using System.Threading.Tasks;
using Elrond.Dotnet.Sdk.Provider;
using Elrond.Dotnet.Sdk.Provider.Dtos;

namespace Elrond.Dotnet.Sdk.Domain
{
    public class GasLimit
    {
        public long Value { get; }

        public GasLimit(long value)
        {
            Value = value;
        }

        public static GasLimit ForTransfer(ConfigResponseDto constants, TransactionRequestDto transaction)
        {
            var value = constants.Data.Config.erd_min_gas_limit;
            if (string.IsNullOrEmpty(transaction.Data)) return new GasLimit(value);
            var bytes = Convert.FromBase64String(transaction.Data);
            value += constants.Data.Config.erd_gas_per_data_byte * bytes.Length;

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