using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Erdcsharp.Domain;
using Erdcsharp.Domain.Exceptions;
using Erdcsharp.Domain.Helper;
using Erdcsharp.Domain.Values;
using Erdcsharp.Provider;
using Erdcsharp.Provider.Dtos;

namespace Erdcsharp.Manager
{
    /// <summary>
    /// Manager to issue and create NFT Token with a provided wallet
    /// </summary>
    public class EsdtTokenManager : IEsdtTokenManager
    {
        private readonly IElrondProvider _provider;
        private readonly NetworkConfig   _networkConfig;

        public EsdtTokenManager(IElrondProvider provider, NetworkConfig networkConfig = null)
        {
            _provider = provider;
            //TODO : Find a better way
            if (networkConfig == null)
                networkConfig = NetworkConfig.GetFromNetwork(provider).GetAwaiter().GetResult();
            _networkConfig = networkConfig;
        }

        public async Task<string> IssueFungibleToken(Wallet wallet, Token token, BigInteger initialSupply)
        {
            var account = wallet.GetAccount();
            await account.Sync(_provider);
            var request = EsdtTokenTransactionRequest.IssueEsdtTransactionRequest(_networkConfig,
                                                                                  account,
                                                                                  token.Name,
                                                                                  token.Ticker,
                                                                                  initialSupply,
                                                                                  token.DecimalPrecision);

            var transaction = await request.Send(_provider, wallet);

            await transaction.AwaitExecuted(_provider);
            transaction.EnsureTransactionSuccess();
            await transaction.AwaitNotarized(_provider);

            var tokenIdentifierValue = transaction.GetSmartContractResult<TokenIdentifierValue>(
                                                                                                TypeValue.TokenIdentifierValue,
                                                                                                1,
                                                                                                1);

            return tokenIdentifierValue.Value;
        }

        public async Task<string> IssueNonFungibleToken(Wallet wallet, string tokenName, string tokenTicker)
        {
            var account = wallet.GetAccount();
            await account.Sync(_provider);
            var request = EsdtTokenTransactionRequest.IssueNonFungibleTokenTransactionRequest(
                                                                                              _networkConfig,
                                                                                              account,
                                                                                              tokenName,
                                                                                              tokenTicker);
            var transaction = await request.Send(_provider, wallet);

            await transaction.AwaitExecuted(_provider);
            transaction.EnsureTransactionSuccess();
            await transaction.AwaitNotarized(_provider);

            var tokenIdentifierValue =
                transaction.GetSmartContractResult<TokenIdentifierValue>(TypeValue.TokenIdentifierValue, 0, 1);
            return tokenIdentifierValue.Value;
        }

        public async Task<string> IssueSemiFungibleToken(Wallet wallet, string tokenName, string tokenTicker)
        {
            var account = wallet.GetAccount();
            await account.Sync(_provider);
            var request = EsdtTokenTransactionRequest.IssueSemiFungibleTokenTransactionRequest(
                                                                                               _networkConfig,
                                                                                               account,
                                                                                               tokenName,
                                                                                               tokenTicker);
            var transaction = await request.Send(_provider, wallet);

            await transaction.AwaitExecuted(_provider);
            transaction.EnsureTransactionSuccess();
            await transaction.AwaitNotarized(_provider);

            var tokenIdentifierValue =
                transaction.GetSmartContractResult<TokenIdentifierValue>(TypeValue.TokenIdentifierValue, 0, 1);
            return tokenIdentifierValue.Value;
        }

        public async Task<List<string>> GetSpecialRole(string tokenIdentifier)
        {
            var response = await _provider.QueryVm(new QueryVmRequestDto
            {
                FuncName  = "getSpecialRoles",
                ScAddress = Constants.SmartContractAddress.EsdtSmartContract,
                Args      = new[] {Converter.ToHexString(TokenIdentifierValue.From(tokenIdentifier).Buffer)}
            });

            return response.Data.ReturnData
                           .Select(Convert.FromBase64String)
                           .Select(decoded => Encoding.UTF8.GetString(decoded))
                           .ToList();
        }

        public async Task SetSpecialRole(Wallet wallet, string tokenIdentifier, params string[] roles)
        {
            var account = wallet.GetAccount();
            await account.Sync(_provider);
            var request = EsdtTokenTransactionRequest.SetSpecialRoleTransactionRequest(
                                                                                       _networkConfig,
                                                                                       account,
                                                                                       account.Address,
                                                                                       tokenIdentifier,
                                                                                       roles
                                                                                      );
            var transaction = await request.Send(_provider, wallet);
            await transaction.AwaitExecuted(_provider);
            transaction.EnsureTransactionSuccess();
            await transaction.AwaitNotarized(_provider);
        }

        public async Task<ulong> CreateNftToken(
            Wallet wallet,
            string tokenIdentifier,
            BigInteger quantity,
            string tokenName,
            ushort royalties,
            Dictionary<string, string> attributes,
            Uri[] uris,
            byte[] hash = null)
        {
            var account = wallet.GetAccount();
            await account.Sync(_provider);
            var request = EsdtTokenTransactionRequest.CreateEsdtNftTokenTransactionRequest(
                                                                                           _networkConfig,
                                                                                           account,
                                                                                           tokenIdentifier,
                                                                                           quantity,
                                                                                           tokenName,
                                                                                           royalties,
                                                                                           hash,
                                                                                           attributes,
                                                                                           uris
                                                                                          );

            var transaction = await request.Send(_provider, wallet);
            await transaction.AwaitExecuted(_provider);
            transaction.EnsureTransactionSuccess();
            await transaction.AwaitNotarized(_provider);

            var nonce = transaction.GetSmartContractResult<NumericValue>(TypeValue.U64TypeValue, 0, 1);
            return (ulong)nonce.Number;
        }

        public async Task<IEnumerable<EsdtToken>> GetEsdtTokens(Address address)
        {
            var tokens       = new List<EsdtToken>();
            var esdtNftToken = await _provider.GetEsdtTokens(address.Bech32);
            foreach (var token in esdtNftToken.Esdts)
            {
                var tokenIdentifier = token.Key.Substring(0, token.Key.IndexOf('-') + 7);
                var properties      = await EsdtToken.EsdtTokenProperties.FromNetwork(_provider, tokenIdentifier);
                var esdt            = EsdtToken.From(token.Value, properties);
                tokens.Add(esdt);
            }

            return tokens;
        }

        public async Task<EsdtToken> GetEsdtFungibleToken(Address address, string tokenIdentifier)
        {
            var esdtNftToken = await _provider.GetEsdtToken(address.Bech32, tokenIdentifier);
            var properties   = await EsdtToken.EsdtTokenProperties.FromNetwork(_provider, tokenIdentifier);

            return EsdtToken.From(esdtNftToken, properties);
        }

        public async Task<EsdtToken> GetEsdtNonFungibleToken(Address address, string tokenIdentifier, ulong tokenId)
        {
            var esdtNftToken = await _provider.GetEsdtNftToken(address.Bech32, tokenIdentifier, tokenId);
            var properties   = await EsdtToken.EsdtTokenProperties.FromNetwork(_provider, tokenIdentifier);
            return EsdtToken.From(esdtNftToken, properties);
        }

        public async Task TransferEsdtToken(Wallet wallet, EsdtToken token, Address receiver, BigInteger quantity)
        {
            var account        = wallet.GetAccount();
            var tokenInAccount = await GetEsdtFungibleToken(account.Address, token.TokenIdentifier.Value);
            if (tokenInAccount.Balance < quantity)
                throw new InsufficientFundException(tokenInAccount.TokenIdentifier.Value);

            await account.Sync(_provider);

            TransactionRequest request;
            switch (token.TokenType)
            {
                case EsdtTokenType.FungibleESDT:
                    request = EsdtTokenTransactionRequest.TransferEsdtTransactionRequest(
                                                                                         _networkConfig,
                                                                                         account,
                                                                                         receiver,
                                                                                         token.TokenIdentifier.Value,
                                                                                         quantity
                                                                                        );

                    break;
                case EsdtTokenType.SemiFungibleESDT:
                case EsdtTokenType.NonFungibleESDT:
                    request = EsdtTokenTransactionRequest.TransferEsdtNftTransactionRequest(
                                                                                            _networkConfig,
                                                                                            account,
                                                                                            receiver,
                                                                                            token.TokenIdentifier.Value,
                                                                                            token.TokenData.TokenId,
                                                                                            quantity);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var transaction = await request.Send(_provider, wallet);
            await transaction.AwaitExecuted(_provider);
            transaction.EnsureTransactionSuccess();
            await transaction.AwaitNotarized(_provider);
        }

        public async Task TransferEsdtTokenToSmartContract(
            Wallet wallet,
            EsdtToken token,
            Address smartContract,
            string functionName,
            BigInteger quantity,
            params IBinaryType[] args)
        {
            var account = wallet.GetAccount();
            await account.Sync(_provider);

            TransactionRequest request;
            switch (token.TokenType)
            {
                case EsdtTokenType.FungibleESDT:
                    request = EsdtTokenTransactionRequest.TransferEsdtTransactionRequest(
                                                                                         _networkConfig,
                                                                                         account,
                                                                                         smartContract,
                                                                                         token.TokenIdentifier.Value,
                                                                                         quantity
                                                                                        );
                    break;
                case EsdtTokenType.SemiFungibleESDT:
                case EsdtTokenType.NonFungibleESDT:
                    request = EsdtTokenTransactionRequest.TransferEsdtNftTransactionRequest(
                                                                                            _networkConfig,
                                                                                            account,
                                                                                            smartContract,
                                                                                            token.TokenIdentifier.Value,
                                                                                            token.TokenData.TokenId,
                                                                                            quantity);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var parameters = new List<IBinaryType> {BytesValue.FromUtf8(functionName)};
            parameters.AddRange(args);
            request.AddArgument(parameters.ToArray());
            request.SetGasLimit(new GasLimit(60000000));

            var transaction = await request.Send(_provider, wallet);

            await transaction.AwaitExecuted(_provider);
            transaction.EnsureTransactionSuccess();
            await transaction.AwaitNotarized(_provider);
        }
    }
}
