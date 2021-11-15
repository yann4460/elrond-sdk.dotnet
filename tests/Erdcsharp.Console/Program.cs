using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Numerics;
using System.Threading.Tasks;
using Elrond.Dotnet.Sdk;
using Elrond.Dotnet.Sdk.Domain;
using Elrond.Dotnet.Sdk.Domain.Values;
using Elrond.Dotnet.Sdk.Manager;
using Elrond.Dotnet.Sdk.Provider;
using Elrond.Dotnet.Sdk.Provider.Dtos;
using Microsoft.Extensions.DependencyInjection;

namespace Elrond.SDK.Console
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddElrondProvider(Extension.Network.TestNet);

            var serviceProvider = services.BuildServiceProvider();

            var provider = serviceProvider.GetRequiredService<IElrondProvider>();
            {
                var password = "&KEiHn!rdBTRCPtaF9Bf";
                var address = "erd17rnvj9shx2x9vh2ckw0nf53vvlylj6235lmrhu668rg2c9a8mxjqvjrhq5";
                var keyFile = KeyFile.FromFilePath($"Wallets/{address}.json");
                var wallet = Wallet.DeriveFromKeyFile(keyFile, password);
                var constants = await Constants.GetFromNetwork(provider);

                //var sc = await DeploySmartContract(provider, constants, wallet, "SmartContracts/auction/auction.wasm");

                await CreateNFTTokenThenTransfer(
                    serviceProvider.GetRequiredService<IEsdtTokenManager>(),
                    serviceProvider.GetRequiredService<IElrondProvider>(),
                    wallet);


                await SynchronizingNetworkParameter();
                await SynchronizingAnAccountObject(provider);

                await CreatingValueTransferTransactions(provider, constants, wallet);

                // This sc is already deployed
                var auction = AddressValue.FromBech32("erd1qqqqqqqqqqqqqpgqjc4rtxq4q7ap37ujrud855ydy6rkslu5rdpqsum6wy");
                await QueryAuctionSmartContractWithoutAbi(provider, auction);
                await QueryAuctionSmartContractWithAbi(provider, auction);
                await QueryAuctionSmartContractWithMultiResult(provider, auction);

                var adder = await DeploySmartContract(provider, constants, wallet, "SmartContracts/adder/adder.wasm");
                await QueryAdderSmartContract(provider, constants, wallet, adder);
                await CallAdderSmartContract(provider, constants, wallet, adder);
            }
        }

        private static async Task SynchronizingNetworkParameter()
        {
            System.Console.WriteLine("SynchronizingNetworkParameter");
            var client = new HttpClient {BaseAddress = new Uri("https://testnet-gateway.elrond.com")};
            var provider = new ElrondProvider(client);
            var constants = await Constants.GetFromNetwork(provider);
            System.Console.WriteLine("MinGasPrice {0}", constants.MinGasPrice);
            System.Console.WriteLine("ChainId {0}", constants.ChainId);
            System.Console.WriteLine("GasPerDataByte {0}", constants.GasPerDataByte);
            System.Console.WriteLine("-*-*-*-*-*" + Environment.NewLine);
        }

        private static async Task SynchronizingAnAccountObject(IElrondProvider provider)
        {
            System.Console.WriteLine("SynchronizingAnAccountObject");

            var address = AddressValue.FromBech32("erd1spyavw0956vq68xj8y4tenjpq2wd5a9p2c6j8gsz7ztyrnpxrruqzu66jx");
            var account = new Account(address);
            await account.Sync(provider);

            System.Console.WriteLine("Balance {0}", account.Balance);
            System.Console.WriteLine("Nonce {0}", account.Nonce);

            System.Console.WriteLine("-*-*-*-*-*" + Environment.NewLine);
        }

        private static async Task CreatingValueTransferTransactions(IElrondProvider provider, Constants constants,
            Wallet wallet)
        {
            System.Console.WriteLine("CreatingValueTransferTransactions");

            var sender = wallet.GetAccount();
            var receiver = AddressValue.FromBech32("erd1qyu5wthldzr8wx5c9ucg8kjagg0jfs53s8nr3zpz3hypefsdd8ssycr6th");
            await sender.Sync(provider);

            var transaction =
                TransactionRequest.CreateTransaction(sender, constants, receiver, Balance.EGLD("0.0000054715"));
            transaction.SetData("Hello world !");
            transaction.SetGasLimit(GasLimit.ForTransfer(constants, transaction));

            var transactionResult = await transaction.Send(provider, wallet);
            await transactionResult.WaitForExecution(provider);
            transactionResult.EnsureTransactionSuccess();

            System.Console.WriteLine("TxHash {0}", transactionResult.TxHash);
            System.Console.WriteLine("-*-*-*-*-*" + Environment.NewLine);
        }

        private static async Task QueryAdderSmartContract(IElrondProvider provider, Constants constants, Wallet wallet,
            AddressValue scAddress)
        {
            System.Console.WriteLine("QueryAdderSmartContract");

            var account = wallet.GetAccount();
            await account.Sync(provider);

            var queryTransaction = SmartContract.CreateCallSmartContractTransactionRequest(constants, account,
                scAddress,
                "getSum",
                Balance.Zero());

            var transaction = await queryTransaction.Send(provider, wallet);
            await transaction.WaitForExecution(provider);
            transaction.EnsureTransactionSuccess();

            // Set the type value according to the ABI description (BigInt)
            var result = transaction.GetSmartContractResult(new[] {TypeValue.BigIntTypeValue});
            var numericResult = result[0].ValueOf<NumericValue>().Number;
            System.Console.WriteLine($"getSum result is {numericResult}");

            System.Console.WriteLine("-*-*-*-*-*" + Environment.NewLine);
        }

        private static async Task QueryAuctionSmartContractWithoutAbi(IElrondProvider provider, AddressValue scAddress)
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

            var results = await SmartContract.QuerySmartContract(
                provider,
                scAddress,
                optionValue,
                "getFullAuctionData", TokenIdentifierValue.From("TSTKR-209ea0"), NumericValue.U64Value(3));

            var fullAuctionData = results.ToObject<FullAuctionData>();
            System.Console.WriteLine("PaymentToken.TokenType {0}", fullAuctionData.PaymentToken.TokenType);
            System.Console.WriteLine("PaymentToken.Nonce {0}", fullAuctionData.PaymentToken.Nonce);
            System.Console.WriteLine("MinBid {0}", fullAuctionData.MinBid);
            System.Console.WriteLine("-*-*-*-*-*" + Environment.NewLine);
        }

        private static async Task QueryAuctionSmartContractWithMultiResult(IElrondProvider provider,
            AddressValue scAddress)
        {
            System.Console.WriteLine("QueryAuctionSmartContractWithMultiResult");

            var abiDefinition = await AbiDefinition.FromJsonFilePath("SmartContracts/auction/auction.abi.json");
            var getMinMaxBid = await SmartContract.QuerySmartContractWithAbiDefinition(
                provider,
                scAddress,
                abiDefinition,
                "getMinMaxBid", TokenIdentifierValue.From("TSTKR-209ea0"), NumericValue.U64Value(3));

            // Need to use the value define in the ABI file (Here it's a StructValue)
            var opt = getMinMaxBid.ValueOf<OptionValue>();
            if (opt.IsSet())
            {
                var values = opt.Value.ToJSON();
                System.Console.WriteLine($"getMinMaxBid : {opt}");
            }

            var getDeadline = await SmartContract.QuerySmartContractWithAbiDefinition(
                provider,
                scAddress,
                abiDefinition,
                "getDeadline", TokenIdentifierValue.From("TSTKR-209ea0"), NumericValue.U64Value(3));

            // Need to use the value define in the ABI file (Here it's a StructValue)
            var optDeadline = getDeadline.ValueOf<OptionValue>();
            if (optDeadline.IsSet())
            {
                var deadline = optDeadline.Value.ValueOf<NumericValue>().Number;
                System.Console.WriteLine($"getDeadline : {deadline}");
            }

            System.Console.WriteLine("-*-*-*-*-*" + Environment.NewLine);
        }

        private static async Task QueryAuctionSmartContractWithAbi(IElrondProvider provider, AddressValue scAddress)
        {
            System.Console.WriteLine("QueryAuctionSmartContractWithAbi");

            var abiDefinition = await AbiDefinition.FromJsonFilePath("SmartContracts/auction/auction.abi.json");
            var getFullAuctionData = await SmartContract.QuerySmartContractWithAbiDefinition(
                provider,
                scAddress,
                abiDefinition,
                "getFullAuctionData", TokenIdentifierValue.From("TSTKR-209ea0"), NumericValue.U64Value(3));

            // Need to use the value define in the ABI file (Here it's a StructValue)
            var optFullAuctionData = getFullAuctionData.ValueOf<OptionValue>();
            if (optFullAuctionData.IsSet())
            {
                var fullAuctionData = optFullAuctionData.Value.ValueOf<StructValue>().Fields;
            }

            System.Console.WriteLine($"optFullAuctionData : {optFullAuctionData}");

            var getDeadline = await SmartContract.QuerySmartContractWithAbiDefinition(
                provider,
                scAddress,
                abiDefinition,
                "getDeadline", TokenIdentifierValue.From("TSTKR-209ea0"), NumericValue.U64Value(3));

            // Need to use the value define in the ABI file (Here it's a StructValue)
            var optDeadline = getDeadline.ValueOf<OptionValue>();
            if (optDeadline.IsSet())
            {
                var deadline = optDeadline.Value.ValueOf<NumericValue>().Number;
            }

            System.Console.WriteLine($"optDeadline : {optDeadline}");
            System.Console.WriteLine("-*-*-*-*-*" + Environment.NewLine);
        }

        private static async Task CallAdderSmartContract(IElrondProvider provider, Constants constants, Wallet wallet,
            AddressValue scAddress)
        {
            System.Console.WriteLine("CallAdderSmartContract");
            var account = wallet.GetAccount();
            await account.Sync(provider);

            // Call 'add' method
            var addRequest = SmartContract.CreateCallSmartContractTransactionRequest(
                constants,
                account,
                scAddress,
                "add",
                Balance.Zero(),
                NumericValue.BigIntValue(12)
            );

            var addRequestTransaction = await addRequest.Send(provider, wallet);
            await addRequestTransaction.WaitForExecution(provider);
            addRequestTransaction.EnsureTransactionSuccess();

            // Query VM
            var result = await provider.QueryVm(new QueryVmRequestDto
            {
                FuncName = "getSum",
                ScAddress = scAddress.Bech32
            });

            var sumBytes = Convert.FromBase64String(result.Data.ReturnData[0]);
            var sumHex = Convert.ToHexString(sumBytes);
            System.Console.WriteLine($"sumHex : {sumHex}");
            System.Console.WriteLine("-*-*-*-*-*" + Environment.NewLine);
        }

        private static async Task<AddressValue> DeploySmartContract(
            IElrondProvider provider,
            Constants constants,
            Wallet wallet,
            string filePath)
        {
            System.Console.WriteLine($"DeploySmartContract : {filePath}");
            var account = wallet.GetAccount();
            await account.Sync(provider);

            var wasmFile = await Code.FromFilePath(filePath);
            var deployRequest = SmartContract.CreateDeploySmartContractTransactionRequest(
                constants,
                account,
                wasmFile,
                new CodeMetadata(false, true, false),
                NumericValue.BigIntValue(5));

            // Get the deployed smart contract address based on account address and transaction nonce
            var smartContractAddress = SmartContract.ComputeAddress(account.Address, account.Nonce);
            var deployTransaction = await deployRequest.Send(provider, wallet);

            await deployTransaction.WaitForExecution(provider);
            deployTransaction.EnsureTransactionSuccess();

            System.Console.WriteLine("-*-*-*-*-*" + Environment.NewLine);

            return smartContractAddress;
        }

        private static async Task<EsdtToken> CreateNFTTokenThenTransfer(
            IEsdtTokenManager tokenManager,
            IElrondProvider provider,
            Wallet wallet)
        {
            System.Console.WriteLine($"[{DateTime.UtcNow:O}] CreateNFTTokenThenTransfer");

            var tokenIdentifier = "STR-94b1f5";
            //{
            //    System.Console.WriteLine($"[{DateTime.UtcNow:O}] IssueNonFungibleToken");
            //    var tokenIdentifier = await tokenManager.IssueNonFungibleToken(wallet, "MyToken2", "MTKN2");
            //    System.Console.WriteLine($"[{DateTime.UtcNow:O}] IssueNonFungibleToken - Result : {tokenIdentifier}");


            //    System.Console.WriteLine($"[{DateTime.UtcNow:O}] SetSpecialRole");
            //    await tokenManager.SetSpecialRole(wallet, tokenIdentifier,
            //        EsdtTokenTransactionRequest.NFTRoles.ESDTRoleNFTCreate);
            //    System.Console.WriteLine($"[{DateTime.UtcNow:O}] SetSpecialRole - Result : Ok ");
            //    var roles = await tokenManager.GetSpecialRole(tokenIdentifier);
            //    foreach (var role in roles)
            //    {
            //        System.Console.WriteLine($"[{DateTime.UtcNow:O}] Roles : " + role);
            //    }
            //}

            System.Console.WriteLine($"[{DateTime.UtcNow:O}] CreateNftToken");
            var token = await tokenManager.CreateNftToken(wallet, tokenIdentifier, "My beautiful token xoxoxoxoxo", 500,
                new Dictionary<string, string>()
                {
                    {"Artist", "Famous artist"},
                    {"Duration", "03.17"}
                },
                new[]
                {
                    new Uri("https://www.google.fr")
                }, Convert.FromHexString("5589558955895589558955895589"));

            System.Console.WriteLine($"[{DateTime.UtcNow:O}] CreateNftToken - Result : {token.TokenId}");

            if (false)
            {
                System.Console.WriteLine($"[{DateTime.UtcNow:O}] TransferNftToken");
                var receiver =
                    AddressValue.FromBech32("erd1spyavw0956vq68xj8y4tenjpq2wd5a9p2c6j8gsz7ztyrnpxrruqzu66jx");
                await tokenManager.TransferEsdtToken(wallet, token, receiver, BigInteger.One);
                System.Console.WriteLine($"[{DateTime.UtcNow:O}] TransferNftToken - Result : Ok");
            }

            var auction = AddressValue.FromBech32("erd1qqqqqqqqqqqqqpgqqfpjhksl9nwruxm6fscs8c85cj6z0dfkmxjqk9pfty");

            var unixTime = (ulong) ((DateTimeOffset) DateTime.Now.AddMinutes(15)).ToUnixTimeSeconds();
            await tokenManager.TransferEsdtTokenToSmartContract(
                wallet,
                token,
                auction,
                "auctionToken",
                BigInteger.One,
                NumericValue.Balance(Balance.EGLD("0.1")),
                NumericValue.Balance(Balance.EGLD("2")),
                NumericValue.U64Value(unixTime),
                TokenIdentifierValue.EGLD());

            var abiDefinition = await AbiDefinition.FromJsonFilePath("SmartContracts/auction/auction.abi.json");
            var getFullAuctionData = await SmartContract.QuerySmartContractWithAbiDefinition(
                provider,
                auction,
                abiDefinition,
                "getFullAuctionData",
                token.TokenIdentifier,
                NumericValue.U64Value(token.TokenId));

            // Need to use the value define in the ABI file (Here it's a StructValue)
            var optFullAuctionData = getFullAuctionData.ValueOf<OptionValue>();
            if (optFullAuctionData.IsSet())
            {
                var fullAuctionData = optFullAuctionData.Value.ValueOf<StructValue>().Fields;
            }

            System.Console.WriteLine("-*-*-*-*-*" + Environment.NewLine);
            return token;
        }
    }
}