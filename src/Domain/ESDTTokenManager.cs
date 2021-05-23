using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
            await Task.Delay(5000);
            transaction.EnsureTransactionSuccess();
        }

        public async Task<ulong> CreateNFT(
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
            return (ulong) tokenId.ValueOf<NumericValue>().Number;
        }

        private async Task<Constants> GetConstants()
        {
            return _constants ??= await Constants.GetFromNetwork(_provider);
        }
    }

    public class NFTToken
    {
        public string TokenIdentifier { get; set; }
        public string Name { get; set; }
        public ulong TokenId { get; set; }
        public string Hash { get; set; }

        public Dictionary<string, string> Attributes { get; set; }

        public int Royalties { get; set; }

        public string Creator { get; set; }
        public string[] Uris { get; set; }
    }
}