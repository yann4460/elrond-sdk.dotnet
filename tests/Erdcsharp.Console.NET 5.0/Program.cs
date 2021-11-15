using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Numerics;
using System.Threading.Tasks;
using Elrond.SDK.Console;
using Erdcsharp.Configuration;
using Erdcsharp.Domain;
using Erdcsharp.Domain.Abi;
using Erdcsharp.Domain.SmartContracts;
using Erdcsharp.Domain.Values;
using Erdcsharp.Manager;
using Erdcsharp.Provider;
using Erdcsharp.Provider.Dtos;

namespace Erdcsharp.Console
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var          provider      = new ElrondProvider(new HttpClient(), new ElrondNetworkConfiguration(Network.TestNet));
            const string password      = "&KEiHn!rdBTRCPtaF9Bf";
            const string address       = "erd17rnvj9shx2x9vh2ckw0nf53vvlylj6235lmrhu668rg2c9a8mxjqvjrhq5";
            var          keyFile       = KeyFile.FromFilePath($"Wallets/{address}.json");
            var          wallet        = Wallet.DeriveFromKeyFile(keyFile, password);
            var          networkConfig = await NetworkConfig.GetFromNetwork(provider);

            //var sc = await DeploySmartContract(provider, networkConfig, wallet, "SmartContracts/auction/auction.wasm");

            await SynchronizingNetworkParameter();
            await SynchronizingAnAccountObject(provider);

            //await CreatingValueTransferTransactions(provider, constants, wallet);

            var token = await CreateNftTokenThenTransfer(
                                                         new EsdtTokenManager(provider, networkConfig),
                                                         provider,
                                                         wallet);

            // This sc is already deployed
            //var auction = await DeploySmartContract(provider, constants, wallet, "SmartContracts/auction/auction.wasm");
            var auction = Address.FromBech32("erd1qqqqqqqqqqqqqpgqlav364jjmeeqezggm7keqy0qgwpute78mxjqzukql8");
            await QueryAuctionSmartContractWithoutAbi(provider, auction, token);
            await QueryAuctionSmartContractWithAbi(provider, auction, token);

            await QueryAuctionSmartContractWithMultiResult(provider, auction);

            var adder = await DeploySmartContract(provider, networkConfig, wallet, "SmartContracts/adder/adder.wasm");
            await QueryAdderSmartContract(provider, networkConfig, wallet, adder);
            await CallAdderSmartContract(provider, networkConfig, wallet, adder);
        }

        private static async Task SynchronizingNetworkParameter()
        {
            System.Console.WriteLine("SynchronizingNetworkParameter");
            var client    = new HttpClient {BaseAddress = new Uri("https://testnet-gateway.elrond.com")};
            var provider  = new ElrondProvider(client);
            var constants = await NetworkConfig.GetFromNetwork(provider);
            System.Console.WriteLine("MinGasPrice {0}", constants.MinGasPrice);
            System.Console.WriteLine("ChainId {0}", constants.ChainId);
            System.Console.WriteLine("GasPerDataByte {0}", constants.GasPerDataByte);
            System.Console.WriteLine("-*-*-*-*-*" + Environment.NewLine);
        }

        private static async Task SynchronizingAnAccountObject(IElrondProvider provider)
        {
            System.Console.WriteLine("SynchronizingAnAccountObject");

            var address = Address.FromBech32("erd1spyavw0956vq68xj8y4tenjpq2wd5a9p2c6j8gsz7ztyrnpxrruqzu66jx");
            var account = new Account(address);
            await account.Sync(provider);

            System.Console.WriteLine("Balance {0}", account.Balance);
            System.Console.WriteLine("Nonce {0}", account.Nonce);

            System.Console.WriteLine("-*-*-*-*-*" + Environment.NewLine);
        }

        private static async Task CreatingValueTransferTransactions(IElrondProvider provider,
                                                                    NetworkConfig networkConfig,
                                                                    Wallet wallet)
        {
            System.Console.WriteLine("CreatingValueTransferTransactions");

            var sender   = wallet.GetAccount();
            var receiver = Address.FromBech32("erd1qyu5wthldzr8wx5c9ucg8kjagg0jfs53s8nr3zpz3hypefsdd8ssycr6th");
            await sender.Sync(provider);

            var transaction =
                TransactionRequest.Create(sender, networkConfig, receiver, TokenAmount.EGLD("0.0000054715"));
            transaction.SetData("Hello world !");
            transaction.SetGasLimit(GasLimit.ForTransfer(networkConfig, transaction));

            var transactionResult = await transaction.Send(provider, wallet);
            await transactionResult.AwaitExecuted(provider);
            transactionResult.EnsureTransactionSuccess();

            System.Console.WriteLine("TxHash {0}", transactionResult.TxHash);
            System.Console.WriteLine("-*-*-*-*-*" + Environment.NewLine);
        }

        private static async Task QueryAdderSmartContract(IElrondProvider provider, NetworkConfig networkConfig,
                                                          Wallet wallet,
                                                          Address scAddress)
        {
            System.Console.WriteLine("QueryAdderSmartContract");

            var account = wallet.GetAccount();
            await account.Sync(provider);

            var queryTransaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig, account,
                                                                                                scAddress,
                                                                                                "getSum",
                                                                                                TokenAmount.Zero());

            var transaction = await queryTransaction.Send(provider, wallet);
            await transaction.AwaitExecuted(provider);
            transaction.EnsureTransactionSuccess();

            // Set the type value according to the ABI description (BigInt)
            var nonce = transaction.GetSmartContractResult<NumericValue>(TypeValue.U64TypeValue, 0, 1);
            System.Console.WriteLine($"getSum result is {nonce}");

            System.Console.WriteLine("-*-*-*-*-*" + Environment.NewLine);
        }

        private static async Task QueryAuctionSmartContractWithoutAbi(IElrondProvider provider, Address scAddress,
                                                                      EsdtToken token)
        {
            System.Console.WriteLine("QueryAuctionSmartContractWithoutAbi");

            var esdtToken = TypeValue.StructValue("EsdtToken", new[]
            {
                new FieldDefinition("token_type", "", TypeValue.TokenIdentifierValue),
                new FieldDefinition("nonce", "", TypeValue.U64TypeValue)
            });

            var auction = TypeValue.StructValue("Auction", new[]
            {
                new FieldDefinition("payment_token", "", esdtToken),
                new FieldDefinition("min_bid", "", TypeValue.BigUintTypeValue),
                new FieldDefinition("max_bid", "", TypeValue.BigUintTypeValue),
                new FieldDefinition("deadline", "", TypeValue.U64TypeValue),
                new FieldDefinition("original_owner", "", TypeValue.AddressValue),
                new FieldDefinition("current_bid", "", TypeValue.BigUintTypeValue),
                new FieldDefinition("current_winner", "", TypeValue.AddressValue),
                new FieldDefinition("marketplace_cut_percentage", "", TypeValue.BigUintTypeValue),
                new FieldDefinition("creator_royalties_percentage", "", TypeValue.BigUintTypeValue)
            });
            var optionValue = TypeValue.OptionValue(auction);

            var results = await SmartContract.QuerySmartContract<OptionValue>(
                                                                              provider,
                                                                              scAddress,
                                                                              optionValue,
                                                                              "getFullAuctionData",
                                                                              null,
                                                                              token.TokenIdentifier,
                                                                              NumericValue.BigUintValue(token.TokenData.TokenId));

            var fullAuctionData = results.ToObject<FullAuctionData>();
            System.Console.WriteLine("PaymentToken.TokenType {0}", fullAuctionData.PaymentToken.TokenType);
            System.Console.WriteLine("PaymentToken.Nonce {0}", fullAuctionData.PaymentToken.Nonce);
            System.Console.WriteLine("MinBid {0}", fullAuctionData.MinBid);
            System.Console.WriteLine("-*-*-*-*-*" + Environment.NewLine);
        }

        private static async Task QueryAuctionSmartContractWithMultiResult(IElrondProvider provider, Address scAddress)
        {
            System.Console.WriteLine("QueryAuctionSmartContractWithMultiResult");

            var abiDefinition = AbiDefinition.FromFilePath("SmartContracts/auction/auction.abi.json");
            var opt = await SmartContract.QuerySmartContractWithAbiDefinition<OptionValue>(
                                                                                           provider,
                                                                                           scAddress,
                                                                                           abiDefinition,
                                                                                           "getMinMaxBid",
                                                                                           null,
                                                                                           TokenIdentifierValue.From("TSTKR-209ea0"),
                                                                                           NumericValue.U64Value(3));

            // Need to use the value define in the ABI file (Here it's a StructValue)
            if (opt.IsSet())
            {
                var values = opt.Value.ToJson();
                System.Console.WriteLine($"getMinMaxBid : {opt}");
            }

            var optDeadline = await SmartContract.QuerySmartContractWithAbiDefinition<OptionValue>(
                                                                                                   provider,
                                                                                                   scAddress,
                                                                                                   abiDefinition,
                                                                                                   "getDeadline",
                                                                                                   null,
                                                                                                   TokenIdentifierValue.From("TSTKR-209ea0"),
                                                                                                   NumericValue.U64Value(3));

            // Need to use the value define in the ABI file (Here it's a StructValue)
            if (optDeadline.IsSet())
            {
                var deadline = optDeadline.Value.ValueOf<NumericValue>().Number;
                System.Console.WriteLine($"getDeadline : {deadline}");
            }

            System.Console.WriteLine("-*-*-*-*-*" + Environment.NewLine);
        }

        private static async Task QueryAuctionSmartContractWithAbi(IElrondProvider provider, Address scAddress,
                                                                   EsdtToken token)
        {
            System.Console.WriteLine("QueryAuctionSmartContractWithAbi");

            var abiDefinition = AbiDefinition.FromFilePath("SmartContracts/auction/auction.abi.json");
            var optFullAuctionData = await SmartContract.QuerySmartContractWithAbiDefinition<OptionValue>(
                                                                                                          provider,
                                                                                                          scAddress,
                                                                                                          abiDefinition,
                                                                                                          "getFullAuctionData",
                                                                                                          null,
                                                                                                          token.TokenIdentifier,
                                                                                                          NumericValue.BigUintValue(token.TokenData.TokenId));

            // Need to use the value define in the ABI file (Here it's a StructValue)
            if (optFullAuctionData.IsSet())
            {
                var fullAuctionData = optFullAuctionData.Value.ValueOf<StructValue>().Fields;
            }

            System.Console.WriteLine($"optFullAuctionData : {optFullAuctionData}");

            var optDeadline = await SmartContract.QuerySmartContractWithAbiDefinition<OptionValue>(
                                                                                                   provider,
                                                                                                   scAddress,
                                                                                                   abiDefinition,
                                                                                                   "getDeadline", null,
                                                                                                   TokenIdentifierValue.From("TSTKR-209ea0"),
                                                                                                   NumericValue.U64Value(3));

            // Need to use the value define in the ABI file (Here it's a StructValue)
            if (optDeadline.IsSet())
            {
                var deadline = optDeadline.Value.ValueOf<NumericValue>().Number;
            }

            System.Console.WriteLine($"optDeadline : {optDeadline}");
            System.Console.WriteLine("-*-*-*-*-*" + Environment.NewLine);
        }

        private static async Task CallAdderSmartContract(IElrondProvider provider, NetworkConfig networkConfig,
                                                         Wallet wallet,
                                                         Address scAddress)
        {
            System.Console.WriteLine("CallAdderSmartContract");
            var account = wallet.GetAccount();
            await account.Sync(provider);

            // Call 'add' method
            var addRequest = TransactionRequest.CreateCallSmartContractTransactionRequest(
                                                                                          networkConfig,
                                                                                          account,
                                                                                          scAddress,
                                                                                          "add",
                                                                                          TokenAmount.Zero(),
                                                                                          NumericValue.BigIntValue(12)
                                                                                         );

            var addRequestTransaction = await addRequest.Send(provider, wallet);
            await addRequestTransaction.AwaitExecuted(provider);
            addRequestTransaction.EnsureTransactionSuccess();

            // Query VM
            var result = await provider.QueryVm(new QueryVmRequestDto
            {
                FuncName  = "getSum",
                ScAddress = scAddress.Bech32
            });

            var sumBytes = Convert.FromBase64String(result.Data.ReturnData[0]);
            var sumHex   = Convert.ToHexString(sumBytes);
            System.Console.WriteLine($"sumHex : {sumHex}");
            System.Console.WriteLine("-*-*-*-*-*" + Environment.NewLine);
        }

        private static async Task<Address> DeploySmartContract(
            IElrondProvider provider,
            NetworkConfig networkConfig,
            Wallet wallet,
            string filePath)
        {
            System.Console.WriteLine($"DeploySmartContract : {filePath}");
            var account = wallet.GetAccount();
            await account.Sync(provider);

            var wasmFile = CodeArtifact.FromFilePath(filePath);
            var deployRequest = TransactionRequest.CreateDeploySmartContractTransactionRequest(
                                                                                               networkConfig,
                                                                                               account,
                                                                                               wasmFile,
                                                                                               new CodeMetadata(false, true, false),
                                                                                               NumericValue.BigIntValue(5));

            // Get the deployed smart contract address based on account address and transaction nonce
            var smartContractAddress = SmartContract.ComputeAddress(account.Address, account.Nonce);
            var deployTransaction    = await deployRequest.Send(provider, wallet);

            await deployTransaction.AwaitExecuted(provider);
            deployTransaction.EnsureTransactionSuccess();

            System.Console.WriteLine("-*-*-*-*-*" + Environment.NewLine);

            return smartContractAddress;
        }

        private static async Task<EsdtToken> CreateNftTokenThenTransfer(
            IEsdtTokenManager tokenManager,
            IElrondProvider provider,
            Wallet wallet)
        {
            System.Console.WriteLine($"[{DateTime.UtcNow:O}] CreateNFTTokenThenTransfer");

            //var tokenIdentifier = "STR-94b1f5";
            System.Console.WriteLine($"[{DateTime.UtcNow:O}] IssueNonFungibleToken");
            var tokenIdentifier = await tokenManager.IssueNonFungibleToken(wallet, "MyToken2", "MTKN2");
            System.Console.WriteLine($"[{DateTime.UtcNow:O}] IssueNonFungibleToken - Result : {tokenIdentifier}");


            System.Console.WriteLine($"[{DateTime.UtcNow:O}] SetSpecialRole");
            await tokenManager.SetSpecialRole(wallet, tokenIdentifier, Constants.EsdtNftSpecialRoles.EsdtRoleNftCreate);
            System.Console.WriteLine($"[{DateTime.UtcNow:O}] SetSpecialRole - Result : Ok ");
            var roles = await tokenManager.GetSpecialRole(tokenIdentifier);
            foreach (var role in roles)
            {
                System.Console.WriteLine($"[{DateTime.UtcNow:O}] Roles : " + role);
            }

            System.Console.WriteLine($"[{DateTime.UtcNow:O}] CreateNftToken");
            var tokenId = await tokenManager.CreateNftToken(wallet, tokenIdentifier,
                                                            BigInteger.One,
                                                            "My beautiful token",
                                                            500,
                                                            new Dictionary<string, string>()
                                                            {
                                                                {"Artist", "Famous artist"},
                                                                {"Duration", "03.17"}
                                                            },
                                                            new[]
                                                            {
                                                                new Uri("https://www.google.fr")
                                                            }, Convert.FromHexString("5589558955895589558955895589"));

            System.Console.WriteLine($"[{DateTime.UtcNow:O}] CreateNftToken - Result : {tokenId}");
            var token = await tokenManager.GetEsdtNonFungibleToken(wallet.GetAccount().Address, tokenIdentifier,
                                                                   tokenId);
            if (false)
            {
                System.Console.WriteLine($"[{DateTime.UtcNow:O}] TransferNftToken");
                var receiver = Address.FromBech32("erd1spyavw0956vq68xj8y4tenjpq2wd5a9p2c6j8gsz7ztyrnpxrruqzu66jx");
                await tokenManager.TransferEsdtToken(wallet, token, receiver, BigInteger.One);
                System.Console.WriteLine($"[{DateTime.UtcNow:O}] TransferNftToken - Result : Ok");
            }

            var auction = Address.FromBech32("erd1qqqqqqqqqqqqqpgqlav364jjmeeqezggm7keqy0qgwpute78mxjqzukql8");

            var unixTime = (ulong) ((DateTimeOffset) DateTime.Now.AddMinutes(15)).ToUnixTimeSeconds();
            await tokenManager.TransferEsdtTokenToSmartContract(
                                                                wallet,
                                                                token,
                                                                auction,
                                                                "auctionToken",
                                                                BigInteger.One,
                                                                NumericValue.TokenAmount(TokenAmount.EGLD("0.1")),
                                                                NumericValue.TokenAmount(TokenAmount.EGLD("2")),
                                                                NumericValue.U64Value(unixTime),
                                                                TokenIdentifierValue.EGLD());

            var abiDefinition = AbiDefinition.FromFilePath("SmartContracts/auction/auction.abi.json");
            var optFullAuctionData = await SmartContract.QuerySmartContractWithAbiDefinition<OptionValue>(
                                                                                                          provider,
                                                                                                          auction,
                                                                                                          abiDefinition,
                                                                                                          "getFullAuctionData",
                                                                                                          null,
                                                                                                          token.TokenIdentifier,
                                                                                                          NumericValue.BigUintValue(token.TokenData.TokenId));

            // Need to use the value define in the ABI file (Here it's a StructValue)
            if (optFullAuctionData.IsSet())
            {
                var fullAuctionData = optFullAuctionData.Value.ValueOf<StructValue>().Fields;
            }

            System.Console.WriteLine("-*-*-*-*-*" + Environment.NewLine);
            return token;
        }
    }
}
