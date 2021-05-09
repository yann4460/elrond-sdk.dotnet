using System.Linq;
using System.Threading.Tasks;
using Elrond.Dotnet.Sdk.Provider;
using Elrond.Dotnet.Sdk.Provider.Dtos;

namespace Elrond.Dotnet.Sdk.Domain
{
    public class Transaction
    {
        public string Status { get; private set; }
        public string SmartContractResult { get; private set; }
        private readonly string _hash;

        private Transaction(string hash, string status)
        {
            Status = status;
            _hash = hash;
        }

        public static Transaction From(TransactionResponseDto transaction)
        {
            return new Transaction(transaction.TxHash, transaction.Status);
        }

        /// <summary>
        /// Returns whether the transaction is pending (e.g. in mempool).
        /// </summary>
        /// <returns></returns>
        public bool IsPending()
        {
            return Status == "received" || Status == "pending" || Status == "partially-executed";
        }

        /// <summary>
        /// Returns whether the transaction has been executed (not necessarily with success)
        /// </summary>
        /// <returns></returns>
        public bool IsExecuted()
        {
            return IsSuccessful() || IsInvalid();
        }

        /// <summary>
        /// Returns whether the transaction has been executed successfully.
        /// </summary>
        /// <returns></returns>
        public bool IsSuccessful()
        {
            return Status == "executed" || Status == "success" || Status == "successful";
        }

        /// <summary>
        /// Returns whether the transaction has been executed, but with a failure.
        /// </summary>
        /// <returns></returns>
        public bool IsFailed()
        {
            return Status == "fail" || Status == "failed" || Status == "unsuccessful" || IsInvalid();
        }

        /// <summary>
        /// Returns whether the transaction has been executed, but marked as invalid (e.g. due to "insufficient funds")
        /// </summary>
        /// <returns></returns>
        public bool IsInvalid()
        {
            return Status == "invalid";
        }


        public T GetSmartContractResult<T>(int index = 0)
        {
            return Argument.GetValue<T>(SmartContractResult, index);
        }


        public async Task Sync(IElrondProvider provider)
        {
            var response = await provider.GetTransactionDetail(_hash);
            Status = response.Status;

            if (response.ScResults != null)
            {
                if (response.ScResults.Any())
                {
                    // Only take the first result atm
                    SmartContractResult = response.ScResults[0].Data;
                }
            }
        }
    }
}