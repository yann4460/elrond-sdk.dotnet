using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Erdcsharp.Domain.Abi;
using Erdcsharp.Domain.Codec;
using Erdcsharp.Domain.Exceptions;
using Erdcsharp.Domain.Helper;
using Erdcsharp.Domain.Values;
using Erdcsharp.Provider;
using Erdcsharp.Provider.Dtos;

namespace Erdcsharp.Domain
{
    public class Transaction
    {
        public string Status { get; private set; }
        public string TxHash { get; }

        private IEnumerable<SmartContractResultDto> _smartContractResult;
        private string                              _hyperBlockHash;

        public Transaction(string hash)
        {
            TxHash = hash;
        }

        public static Transaction From(CreateTransactionResponseDataDto createTransactionResponse)
        {
            return new Transaction(createTransactionResponse.TxHash);
        }

        public T GetSmartContractResult<T>(TypeValue type, int smartContractIndex = 0, int parameterIndex = 0)
            where T : IBinaryType
        {
            if (!_smartContractResult.Any())
                throw new Exception("Empty smart contract results");

            var scResult = _smartContractResult.ElementAt(smartContractIndex).Data;

            var fields          = scResult.Split('@').Where(s => !string.IsNullOrEmpty(s)).ToArray();
            var result          = fields.ElementAt(parameterIndex);
            var responseBytes   = Converter.FromHexString(result);
            var binaryCodec     = new BinaryCodec();
            var decodedResponse = binaryCodec.DecodeTopLevel(responseBytes, type);
            return (T)decodedResponse;
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

        public async Task Sync(IElrondProvider provider)
        {
            var transaction = await provider.GetTransactionDetail(TxHash);
            if (transaction.SmartContractResults != null)
            {
                _smartContractResult = transaction.SmartContractResults.OrderByDescending(s => s.Nonce).ToList();
            }

            _hyperBlockHash = transaction.HyperblockHash;
            Status          = transaction.Status;
        }

        public void EnsureTransactionSuccess()
        {
            if (!IsSuccessful())
                throw new TransactionException.InvalidTransactionException(TxHash);
        }

        /// <summary>
        /// Wait for the execution of the transaction
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task AwaitExecuted(IElrondProvider provider, TimeSpan? timeout = null)
        {
            if (!timeout.HasValue)
                timeout = TimeSpan.FromSeconds(60);

            var currentIteration = 0;
            do
            {
                await Task.Delay(1000); // 1 second
                await Sync(provider);
                currentIteration++;
            } while (!IsExecuted() && currentIteration < timeout.Value.TotalSeconds);

            if (!IsExecuted())
                throw new TransactionException.TransactionStatusNotReachedException(TxHash, "Executed");

            if (_smartContractResult != null && _smartContractResult.Any(s => !string.IsNullOrEmpty(s.ReturnMessage)))
            {
                var returnMessages   = _smartContractResult.Select(x => x.ReturnMessage).ToArray();
                var aggregateMessage = string.Join(Environment.NewLine, returnMessages);
                throw new TransactionException.TransactionWithSmartContractErrorException(TxHash, aggregateMessage);
            }

            if (IsFailed())
                throw new TransactionException.FailedTransactionException(TxHash);

            if (IsInvalid())
                throw new TransactionException.InvalidTransactionException(TxHash);
        }

        /// <summary>
        /// Wait for the transaction to be notarized
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task AwaitNotarized(IElrondProvider provider, TimeSpan? timeout = null)
        {
            if (!timeout.HasValue)
                timeout = TimeSpan.FromSeconds(60);

            var currentIteration = 0;
            do
            {
                await Task.Delay(1000); // 1 second
                await Sync(provider);
                currentIteration++;
            } while (string.IsNullOrEmpty(_hyperBlockHash) && currentIteration < timeout.Value.TotalSeconds);

            if (currentIteration >= timeout.Value.TotalSeconds)
                throw new TransactionException.TransactionStatusNotReachedException(TxHash, "Notarized");
        }
    }
}
