using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elrond.Dotnet.Sdk.Domain.Codec;
using Elrond.Dotnet.Sdk.Domain.Values;
using Elrond.Dotnet.Sdk.Provider;
using Elrond.Dotnet.Sdk.Provider.Dtos;

namespace Elrond.Dotnet.Sdk.Domain
{
    public class Transaction
    {
        public string Status { get; private set; }
        public string TxHash { get; }

        //TODO : Check smart contract result to compute status
        private SmartContractResultDto[] _smartContractResult;

        public Transaction(string hash)
        {
            TxHash = hash;
        }

        public static Transaction From(CreateTransactionResponseDto createTransactionResponse)
        {
            return new Transaction(createTransactionResponse.Data.TxHash);
        }

        public List<IBinaryType> GetSmartContractResult(string endpoint, AbiDefinition abiDefinition)
        {
            if (_smartContractResult == null || _smartContractResult.Length == 0)
                throw new Exception("Empty smart contract results");

            var binaryCodec = new BinaryCodec();
            var endpointDefinition = abiDefinition.GetEndpointDefinition(endpoint);

            var decodedResponses = new List<IBinaryType>();
            var firstScResult = _smartContractResult[0].Data;
            var resultFieldsHex = firstScResult.Split('@', StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray();
            for (var i = 0; i < endpointDefinition.Output.Length; i++)
            {
                var output = endpointDefinition.Output[i];
                var responseBytes = Convert.FromHexString(resultFieldsHex[i]);
                var decodedResponse = binaryCodec.DecodeTopLevel(responseBytes, output.Type);
                decodedResponses.Add(decodedResponse);
            }

            return decodedResponses;
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
            var detail = await provider.GetTransactionDetail(TxHash);
            var transaction = detail.Data.Transaction;
            _smartContractResult = transaction.SmartContractResults;
            Status = transaction.Status;
        }
    }
}