using System;
using System.Threading.Tasks;
using Erdcsharp.Provider;

namespace Erdcsharp.Domain
{
    public class GasLimit
    {
        public long Value { get; }

        public GasLimit(long value)
        {
            Value = value;
        }

        /// <summary>
        /// Compute GasLimit for transaction
        /// </summary>
        /// <param name="networkConfig">The network config</param>
        /// <param name="transaction">The transaction</param>
        /// <returns>A GasLimit</returns>
        public static GasLimit ForTransfer(NetworkConfig networkConfig, TransactionRequest transaction)
        {
            var value = networkConfig.MinGasLimit;
            if (string.IsNullOrEmpty(transaction.Data)) return new GasLimit(value);
            var bytes = Convert.FromBase64String(transaction.Data);
            value += networkConfig.GasPerDataByte * bytes.Length;

            return new GasLimit(value);
        }

        /// <summary>
        /// Compute GasLimit for a smat contract call
        /// </summary>
        /// <param name="networkConfig">The network config</param>
        /// <param name="transaction">The transaction</param>
        /// <returns>A GasLimit</returns>
        public static GasLimit ForSmartContractCall(NetworkConfig networkConfig, TransactionRequest transaction)
        {
            var value = networkConfig.MinGasLimit + 6000000;
            if (string.IsNullOrEmpty(transaction.Data))
                return new GasLimit(value);

            var bytes = Convert.FromBase64String(transaction.Data);
            value += networkConfig.GasPerDataByte * bytes.Length;

            return new GasLimit(value);
        }

        public static async Task<GasLimit> ForTransaction(TransactionRequest transactionRequest, IElrondProvider provider)
        {
            var cost = await provider.GetTransactionCost(transactionRequest.GetTransactionRequest());
            if (cost.TxGasUnits == 0)
                throw new Exception($"Unable to get cost of transaction : {cost.ReturnMessage}");

            return new GasLimit(cost.TxGasUnits);
        }
    }
}
