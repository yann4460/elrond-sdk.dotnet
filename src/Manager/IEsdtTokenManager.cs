using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Erdcsharp.Domain;
using Erdcsharp.Domain.Values;

namespace Erdcsharp.Manager
{
    public interface IEsdtTokenManager
    {
        /// <summary>
        /// ESDT tokens are issued via a request to the Metachain,
        /// which is a transaction submitted by the Account which will manage the tokens.
        /// When issuing a token, one must provide a token name, a ticker, the initial supply,
        /// the number of decimals for display purpose and optionally additional properties
        /// </summary>
        /// <param name="wallet"></param>
        /// <param name="token"></param>
        /// <param name="initialSupply">Initial supply, should have an even number of characters</param>
        /// <returns>The token identifier</returns>
        Task<string> IssueFungibleToken(Wallet wallet, Token token, BigInteger initialSupply);

        /// <summary>
        /// One has to perform an issuance transaction in order to register a non fungible token.
        /// Non FungibleESDT Tokens are issued via a request to the Metachain, which is a transaction submitted by the Account which will manage the tokens.
        ///  When issuing a token, one must provide a token name, a ticker and optionally additional properties.
        /// </summary>
        /// <param name="wallet"></param>
        /// <param name="tokenName"></param>
        /// <param name="tokenTicker"></param>
        /// <returns></returns>
        Task<string> IssueNonFungibleToken(Wallet wallet, string tokenName, string tokenTicker);

        /// <summary>
        /// One has to perform an issuance transaction in order to register a semi fungible token.
        /// Semi FungibleESDT Tokens are issued via a request to the Metachain, which is a transaction submitted by the Account which will manage the tokens
        /// When issuing a semi fungible token, one must provide a token name, a ticker, the initial quantity and optionally additional properties
        /// </summary>
        /// <param name="wallet"></param>
        /// <param name="tokenName"></param>
        /// <param name="tokenTicker"></param>
        /// <returns></returns>
        Task<string> IssueSemiFungibleToken(Wallet wallet, string tokenName, string tokenTicker);

        Task<List<string>> GetSpecialRole(string tokenIdentifier);

        Task SetSpecialRole(Wallet wallet, string tokenIdentifier, params string[] roles);

        Task<ulong> CreateNftToken(
            Wallet wallet,
            string tokenIdentifier,
            BigInteger quantity,
            string tokenName,
            ushort royalties,
            Dictionary<string, string> attributes,
            Uri[] uris,
            byte[] hash = null);

        Task<IEnumerable<EsdtToken>> GetEsdtTokens(Address address);

        Task<EsdtToken> GetEsdtFungibleToken(Address address, string tokenIdentifier);

        Task<EsdtToken> GetEsdtNonFungibleToken(Address address, string tokenIdentifier, ulong tokenId);

        Task TransferEsdtToken(Wallet wallet, EsdtToken token, Address receiver, BigInteger quantity);

        Task TransferEsdtTokenToSmartContract(Wallet wallet, EsdtToken token, Address smartContract,
                                              string functionName, BigInteger quantity, params IBinaryType[] args);
    }
}
