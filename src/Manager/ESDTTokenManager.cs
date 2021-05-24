using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elrond.Dotnet.Sdk.Domain;
using Elrond.Dotnet.Sdk.Domain.Values;
using Elrond.Dotnet.Sdk.Provider;
using Elrond.Dotnet.Sdk.Provider.Dtos;

namespace Elrond.Dotnet.Sdk.Manager
{
    /// <summary>
    /// Manager to issue and create NFT Token with a provided wallet
    /// </summary>
    public class EsdtTokenManager : IEsdtTokenManager
    {
        private readonly IElrondProvider _provider;
        private Constants _constants;

        public EsdtTokenManager(IElrondProvider provider)
        {
            _provider = provider;
        }

        public async Task<string> IssueNonFungibleToken(Wallet wallet, string tokenName, string tokenTicker)
        {
            var account = wallet.GetAccount();
            var constants = await GetConstants();
            await account.Sync(_provider);
            var request = EsdtTokenTransactionRequest.IssueNonFungibleTokenTransactionRequest(
                constants,
                account,
                tokenName,
                tokenTicker);
            var transaction = await request.Send(_provider, wallet);

            await transaction.WaitForExecution(_provider);
            transaction.EnsureTransactionSuccess();

            var tokenIdentifierValue =
                transaction.GetSmartContractResult(new[] {TypeValue.TokenIdentifierValue}).Single();
            return tokenIdentifierValue.ValueOf<TokenIdentifierValue>().TokenIdentifier;
        }

        public async Task<List<string>> GetSpecialRole(string tokenIdentifier)
        {
            var response = await _provider.QueryVm(new QueryVmRequestDto
            {
                FuncName = "getSpecialRoles",
                ScAddress = "erd1qqqqqqqqqqqqqqqpqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqzllls8a5w6u",
                Args = new[]
                {
                    Convert.ToHexString(TokenIdentifierValue.From(tokenIdentifier).Buffer)
                }
            });

            return response.Data.ReturnData.Select(Convert.FromBase64String).Select(decoded => Encoding.UTF8.GetString(decoded)).ToList();
        }

        public async Task SetSpecialRole(Wallet wallet, string tokenIdentifier, params string[] roles)
        {
            var account = wallet.GetAccount();
            var constants = await GetConstants();
            await account.Sync(_provider);
            var request = EsdtTokenTransactionRequest.SetSpecialRoleTransactionRequest(
                constants,
                account,
                account.Address,
                tokenIdentifier,
                roles
            );
            var transaction = await request.Send(_provider, wallet);
            await transaction.WaitForExecution(_provider);
            transaction.EnsureTransactionSuccess();
        }

        public async Task<EsdtToken> CreateNftToken(
            Wallet wallet,
            string tokenIdentifier,
            string tokenName,
            ushort royalties,
            Dictionary<string, string> attributes,
            Uri[] uris,
            string hash = null)
        {
            var account = wallet.GetAccount();
            var constants = await GetConstants();
            await account.Sync(_provider);
            var request = EsdtTokenTransactionRequest.CreateEsdtNftTokenTransactionRequest(
                constants,
                account,
                tokenIdentifier,
                tokenName,
                royalties,
                hash,
                attributes,
                uris
            );

            var transaction = await request.Send(_provider, wallet);
            await transaction.WaitForExecution(_provider);
            transaction.EnsureTransactionSuccess();

            var tokenId = transaction.GetSmartContractResult(new[] {TypeValue.U64TypeValue}).Single();
            var nonce = (ulong) tokenId.ValueOf<NumericValue>().Number;

            return new EsdtToken
            {
                Name = tokenName,
                TokenIdentifier = TokenIdentifierValue.From(tokenIdentifier),
                Creator = account.Address,
                TokenId = nonce,
                Royalties = royalties,
                Attributes = attributes,
                Hash = hash,
                Uris = uris
            };
        }

        public async Task<EsdtToken> GetNftToken(AddressValue address, string tokenIdentifier, ulong tokenId)
        {
            var esdtNftToken = await _provider.GetEsdtNftToken(address.Bech32, tokenIdentifier, tokenId);
            return EsdtToken.From(esdtNftToken);
        }

        public async Task TransferNftToken(Wallet wallet, EsdtToken token, AddressValue receiver)
        {
            var account = wallet.GetAccount();
            var constants = await GetConstants();
            await account.Sync(_provider);
            var request = EsdtTokenTransactionRequest.TransferEsdtNftTransactionRequest(
                constants,
                account,
                receiver,
                token.TokenIdentifier.TokenIdentifier,
                token.TokenId,
                1
            );

            var transaction = await request.Send(_provider, wallet);

            await transaction.WaitForExecution(_provider);
            transaction.EnsureTransactionSuccess();
        }

        private async Task<Constants> GetConstants()
        {
            return _constants ??= await Constants.GetFromNetwork(_provider);
        }
    }
}