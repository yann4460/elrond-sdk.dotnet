using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Elrond.Dotnet.Sdk.Domain.Values;
using Elrond.Dotnet.Sdk.Provider;

namespace Elrond.Dotnet.Sdk.Domain
{
    public class ESDTTokenManager
    {
        private readonly IElrondProvider _provider;
        private readonly Constants _constants;
        private readonly Wallet _wallet;
        private readonly Account _account;

        public ESDTTokenManager(IElrondProvider provider, Constants constants, Wallet wallet)
        {
            _provider = provider;
            _constants = constants;
            _wallet = wallet;
            _account = wallet.GetAccount();
        }

        public async Task<string> IssueNonFungibleToken(string tokenName, string tokenTicker)
        {
            await _account.Sync(_provider);
            var request = ESDTTokenTransactionRequest.IssueNonFungibleTokenTransactionRequest(
                _constants,
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
            await _account.Sync(_provider);
            var request = ESDTTokenTransactionRequest.SetSpecialRoleTransactionRequest(
                _constants,
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

        public async Task<ulong> CreateNFT(string tokenIdentifier, string tokenName)
        {
            await _account.Sync(_provider);
            var request = ESDTTokenTransactionRequest.CreateESDTNFTTokenTransactionRequest(
                _constants,
                _account,
                tokenIdentifier,
                BigInteger.One,
                tokenName,
                100,
                "",
                new Dictionary<string, string>
                {
                    {"Origin", "My custom origin"},
                    {"Origin1", "My custom origin"},
                    {"Origin2", "My custom origin"},
                    {"Origin3", "My custom origin"}
                },
                new[]
                {
                    "https://www.google.fr",
                    "https://www.google.fr",
                    "https://www.google.fr",
                }
            );

            var transaction = await request.Send(_provider, _wallet);
            await transaction.WaitForExecution(_provider);
            transaction.EnsureTransactionSuccess();

            var tokenId = transaction.GetSmartContractResult(new[] {TypeValue.U64TypeValue}).Single();
            return (ulong) tokenId.ValueOf<NumericValue>().Number;
        }
    }
}