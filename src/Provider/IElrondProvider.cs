using System.Threading.Tasks;
using Erdcsharp.Provider.Dtos;

namespace Erdcsharp.Provider
{
    public interface IElrondProvider
    {
        /// <summary>
        /// This endpoint allows one to query basic details about the configuration of the Network.
        /// </summary>
        /// <returns><see cref="ConfigDataDto"/></returns>
        Task<ConfigDataDto> GetNetworkConfig();

        /// <summary>
        /// This endpoint allows one to retrieve basic information about an Address (Account).
        /// </summary>
        /// <param name="address">The address</param>
        /// <returns><see cref="AccountDataDto"/></returns>
        Task<AccountDto> GetAccount(string address);

        /// <summary>
        /// Returns an array of FungibleESDT Tokens that the specified address has interacted with (issued, sent or received).
        /// </summary>
        /// <param name="address">The Address to query in bech32 format.</param>
        /// <returns><see cref="EsdtTokenDataDto"/></returns>
        Task<EsdtTokenDataDto> GetEsdtTokens(string address);

        /// <summary>
        /// Returns the balance of an address for specific FungibleESDT Tokens.
        /// </summary>
        /// <param name="address">The Address to query in bech32 format.</param>
        /// <param name="tokenIdentifier">The token identifier.</param>
        /// <param name="tokenId">The nonce after the NFT creation..</param>
        /// <returns></returns>
        Task<EsdtItemDto> GetEsdtNftToken(string address, string tokenIdentifier, ulong tokenId);

        /// <summary>
        /// Returns the balance of an address for specific ESDT Tokens.
        /// </summary>
        /// <param name="address">The Address to query in bech32 format.</param>
        /// <param name="tokenIdentifier">The token identifier.</param>
        /// <returns><see cref="EsdtTokenData"/></returns>
        Task<EsdtTokenData> GetEsdtToken(string address, string tokenIdentifier);

        /// <summary>
        /// This endpoint allows one to send a signed Transaction to the Blockchain.
        /// </summary>
        /// <param name="transactionRequestDto">The transaction payload</param>
        /// <returns></returns>
        Task<CreateTransactionResponseDataDto> SendTransaction(TransactionRequestDto transactionRequestDto);

        /// <summary>
        /// This endpoint allows one to query the details of a Transaction.
        /// </summary>
        /// <param name="txHash">The transaction hash</param>
        /// <returns><see cref="TransactionDto"/></returns>
        Task<TransactionDto> GetTransactionDetail(string txHash);

        /// <summary>
        /// This endpoint allows one to estimate the cost of a transaction.
        /// </summary>
        /// <param name="transactionRequestDto">The transaction payload</param>
        /// <returns><see cref="TransactionCostDataDto"/></returns>
        Task<TransactionCostDataDto> GetTransactionCost(TransactionRequestDto transactionRequestDto);

        /// <summary>
        /// This endpoint allows one to execute - with no side-effects - a pure function of a Smart Contract and retrieve the execution results (the Virtual Machine Output).
        /// </summary>
        /// <param name="queryVmRequestDto"></param>
        /// <returns><see cref="QueryVmResultDataDto"/></returns>
        Task<QueryVmResultDataDto> QueryVm(QueryVmRequestDto queryVmRequestDto);
    }
}
