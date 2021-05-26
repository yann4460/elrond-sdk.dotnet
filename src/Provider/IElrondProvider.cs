using System.Threading.Tasks;
using Elrond.Dotnet.Sdk.Provider.Dtos;

namespace Elrond.Dotnet.Sdk.Provider
{
    public interface IElrondProvider
    {
        Task<ConfigDataDto> GetConstants();

        Task<AccountDataDto> GetAccount(string address);


        /// <summary>
        /// Returns an array of ESDT Tokens that the specified address has interacted with (issued, sent or received).
        /// </summary>
        /// <param name="address">The Address to query in bech32 format.</param>
        /// <returns><see cref="ESDTTokenDataDto"/></returns>
        Task<ESDTTokenDataDto> GetEsdtTokens(string address);

        /// <summary>
        /// Returns the balance of an address for specific ESDT Tokens.
        /// </summary>
        /// <param name="address">The Address to query in bech32 format.</param>
        /// <param name="tokenIdentifier">The token identifier.</param>
        /// <param name="tokenId">The nonce after the NFT creation..</param>
        /// <returns></returns>
        Task<EsdtNftItemDto> GetEsdtNftToken(string address, string tokenIdentifier, ulong tokenId);

        Task<EsdtDataDto> GetEsdtToken(string address, string tokenIdentifier);

        Task<CreateTransactionResponseDataDto> SendTransaction(TransactionRequestDto transactionRequestDto);

        Task SimulateTransaction(TransactionRequestDto transactionRequestDto);

        Task<TransactionResponseData> GetTransactionDetail(string txHash);

        Task<TransactionCostDataDto> GetTransactionCost(TransactionRequestDto transactionRequestDto);

        Task<QueryVmResultDataDto> QueryVm(QueryVmRequestDto queryVmRequestDto);
    }
}