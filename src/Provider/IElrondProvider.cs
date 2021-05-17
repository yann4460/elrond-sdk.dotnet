using System.Collections.Generic;
using System.Threading.Tasks;
using Elrond.Dotnet.Sdk.Provider.Dtos;

namespace Elrond.Dotnet.Sdk.Provider
{
    public interface IElrondProvider
    {
        Task<ConfigResponseDto> GetConstants();

        Task<AccountResponseDto> GetAccount(string address);

        Task<IReadOnlyCollection<ESDTTokenResponseDto>> GetESDTTokens(string address);

        Task<CreateTransactionResponseDto> SendTransaction(TransactionRequestDto transactionRequestDto);

        Task SimulateTransaction(TransactionRequestDto transactionRequestDto);

        Task<TransactionResponseDto> GetTransactionDetail(string txHash);

        Task<TransactionCostDto> GetTransactionCost(TransactionRequestDto transactionRequestDto);

        Task<QueryVmResultDto> QueryVm(QueryVmRequestDto queryVmRequestDto);
    }
}