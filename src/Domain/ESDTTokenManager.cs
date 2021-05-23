using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elrond.Dotnet.Sdk.Domain.Values;
using Elrond.Dotnet.Sdk.Provider;

namespace Elrond.Dotnet.Sdk.Domain
{
    public class EsdtTokenManager
    {
        private readonly IElrondProvider _provider;
        private Constants _constants;
        private readonly Wallet _wallet;
        private readonly Account _account;

        public EsdtTokenManager(IElrondProvider provider, Wallet wallet)
        {
            _provider = provider;
            _wallet = wallet;
            _account = wallet.GetAccount();
        }

        public async Task<string> IssueNonFungibleToken(string tokenName, string tokenTicker)
        {
            var constants = await GetConstants();
            await _account.Sync(_provider);
            var request = ESDTTokenTransactionRequest.IssueNonFungibleTokenTransactionRequest(
                constants,
                _account,
                tokenName,
                tokenTicker);
            var transaction = await request.Send(_provider, _wallet);

            await transaction.WaitForExecution(_provider);
            transaction.EnsureTransactionSuccess();

            var tokenIdentifierValue =
                transaction.GetSmartContractResult(new[] {TypeValue.TokenIdentifierValue}).Single();
            return tokenIdentifierValue.ValueOf<TokenIdentifierValue>().TokenIdentifier;
        }

        public async Task SetSpecialRole(string tokenIdentifier, params string[] roles)
        {
            var constants = await GetConstants();
            await _account.Sync(_provider);
            var request = ESDTTokenTransactionRequest.SetSpecialRoleTransactionRequest(
                constants,
                _account,
                _account.Address,
                tokenIdentifier,
                roles
            );
            var transaction = await request.Send(_provider, _wallet);

            await transaction.WaitForExecution(_provider);
            await Task.Delay(2000);
            transaction.EnsureTransactionSuccess();
        }

        public async Task<EstdToken> CreateNFTToken(
            string tokenIdentifier,
            string tokenName,
            ushort royalties,
            Dictionary<string, string> attributes,
            Uri[] uris,
            string hash = null)
        {
            var constants = await GetConstants();
            await _account.Sync(_provider);
            var request = ESDTTokenTransactionRequest.CreateESDTNFTTokenTransactionRequest(
                constants,
                _account,
                tokenIdentifier,
                tokenName,
                royalties,
                hash,
                attributes,
                uris
            );

            var transaction = await request.Send(_provider, _wallet);
            await transaction.WaitForExecution(_provider);
            transaction.EnsureTransactionSuccess();

            var tokenId = transaction.GetSmartContractResult(new[] {TypeValue.U64TypeValue}).Single();
            var nonce = (ulong) tokenId.ValueOf<NumericValue>().Number;
           
            return new EstdToken
            {
                Name = tokenName,
                TokenIdentifier = TokenIdentifierValue.From(tokenIdentifier),
                Creator = _account.Address,
                TokenId = nonce,
                Royalties = royalties,
                Attributes = attributes,
                Hash = hash,
                Uris = uris
            };
        }

        private async Task<Constants> GetConstants()
        {
            return _constants ??= await Constants.GetFromNetwork(_provider);
        }
    }
}