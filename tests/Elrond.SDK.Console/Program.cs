using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Elrond.Dotnet.Sdk.Domain;
using Elrond.Dotnet.Sdk.Domain.Values;
using Elrond.Dotnet.Sdk.Provider;
using Elrond.Dotnet.Sdk.Provider.Dtos;

namespace Elrond.SDK.Console
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var testPrivateKey =
                "C5A89BFA5E8FFFA4BAA732D8D8EE9503FAFA538599C3DDEE28D21F64DFFDBF00FDB32E9ED34CAF6009834C5A5BEF293097EA39698B3E82EFD8C71183CB731B42";

            var client = new HttpClient {BaseAddress = new Uri("https://testnet-gateway.elrond.com")};
            var provider = new ElrondProvider(client);
            var wallet = new Wallet(testPrivateKey);
            var constants = await Constants.GetFromNetwork(provider);

            await SynchronizingNetworkParameter();
            await SynchronizingAnAccountObject(provider);

            await CreatingValueTransferTransactions(provider, constants, wallet);

            var auctionScAddress =
                AddressValue.FromBech32("erd1qqqqqqqqqqqqqpgqjc4rtxq4q7ap37ujrud855ydy6rkslu5rdpqsum6wy");
            await QueryAuctionSmartContractWithoutAbi(provider, auctionScAddress);
            await QueryAuctionSmartContractWithAbi(provider, auctionScAddress);

            var adderScAddress =
                await DeploySmartContract(provider, constants, wallet, "SmartContracts/adder/adder.wasm");
            await QueryAdderSmartContract(provider, constants, wallet, adderScAddress);
            await CallAdderSmartContract(provider, constants, wallet, adderScAddress);
        }

        private static async Task SynchronizingNetworkParameter()
        {
            var client = new HttpClient {BaseAddress = new Uri("https://testnet-gateway.elrond.com")};
            var provider = new ElrondProvider(client);
            var constants = await Constants.GetFromNetwork(provider);
            System.Console.WriteLine("MinGasPrice {0}", constants.MinGasPrice);
            System.Console.WriteLine("ChainId {0}", constants.ChainId);
            System.Console.WriteLine("GasPerDataByte {0}", constants.GasPerDataByte);
        }

        private static async Task SynchronizingAnAccountObject(IElrondProvider provider)
        {
            var address = AddressValue.FromBech32("erd1spyavw0956vq68xj8y4tenjpq2wd5a9p2c6j8gsz7ztyrnpxrruqzu66jx");
            var account = new Account(address);
            await account.Sync(provider);

            System.Console.WriteLine("Balance {0}", account.Balance);
            System.Console.WriteLine("Nonce {0}", account.Nonce);
        }

        private static async Task CreatingValueTransferTransactions(IElrondProvider provider, Constants constants,
            Wallet wallet)
        {
            var sender = wallet.GetAccount();
            var receiver = AddressValue.FromBech32("erd1qyu5wthldzr8wx5c9ucg8kjagg0jfs53s8nr3zpz3hypefsdd8ssycr6th");
            await sender.Sync(provider);

            var transaction = TransactionRequest.CreateTransaction(sender, constants, receiver, Balance.EGLD("2.15"));

            transaction.SetData("Hello world !");
            transaction.SetGasLimit(GasLimit.ForTransfer(constants, transaction));

            var transactionResult = await transaction.Send(wallet, provider);
            System.Console.WriteLine("TxHash {0}", transactionResult.TxHash);
        }

        private static async Task QueryAdderSmartContract(IElrondProvider provider, Constants constants, Wallet wallet,
            AddressValue scAddress)
        {
            var account = wallet.GetAccount();
            await account.Sync(provider);

            var queryTransaction = SmartContract.CreateCallSmartContractTransactionRequest(constants, account,
                scAddress,
                "getSum",
                Balance.Zero());

            queryTransaction.SetGasLimit(await GasLimit.ForTransaction(queryTransaction, provider));

            var transaction = await queryTransaction.Send(wallet, provider);
            await transaction.WaitForExecution(provider);
            transaction.EnsureTransactionSuccess();

            // Set the type value according to the ABI description (BigInt)
            var result = transaction.GetSmartContractResult(new[] {TypeValue.BigIntTypeValue});
            var numericResult = result[0].ValueOf<NumericValue>().Number;
        }

        private static async Task QueryAuctionSmartContractWithoutAbi(IElrondProvider provider, AddressValue scAddress)
        {
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
            var option = TypeValue.OptionValue(auction);

            var results = await SmartContract.QuerySmartContract(
                provider,
                scAddress,
                new[] {option},
                "getFullAuctionData", TokenIdentifierValue.From("TSTKR-209ea0"), NumericValue.U64Value(3));

            var fullAuctionData = results[0].ToObject<FullAuctionData>();
            System.Console.WriteLine("payment_token.token_type {0}", fullAuctionData.payment_token.token_type);
            System.Console.WriteLine("payment_token.nonce {0}", fullAuctionData.payment_token.nonce);
            System.Console.WriteLine("min_bid {0}", fullAuctionData.min_bid);
        }

        private static async Task QueryAuctionSmartContractWithAbi(IElrondProvider provider, AddressValue scAddress)
        {
            var abiDefinition = await AbiDefinition.FromJsonFilePath("SmartContracts/auction/auction.abi.json");
            var getFullAuctionData = await SmartContract.QuerySmartContractWithAbiDefinition(
                provider,
                scAddress,
                abiDefinition,
                "getFullAuctionData", TokenIdentifierValue.From("TSTKR-209ea0"), NumericValue.U64Value(3));

            // Need to use the value define in the ABI file (Here it's a StructValue)
            var optFullAuctionData = getFullAuctionData[0].ValueOf<OptionValue>();
            if (optFullAuctionData.IsSet())
            {
                var fullAuctionData = optFullAuctionData.Value.ValueOf<StructValue>().Fields;
            }

            var getDeadline = await SmartContract.QuerySmartContractWithAbiDefinition(
                provider,
                scAddress,
                abiDefinition,
                "getDeadline", TokenIdentifierValue.From("TSTKR-209ea0"), NumericValue.U64Value(3));

            // Need to use the value define in the ABI file (Here it's a StructValue)
            var optDeadline = getDeadline[0].ValueOf<OptionValue>();
            if (optDeadline.IsSet())
            {
                var deadline = optDeadline.Value.ValueOf<NumericValue>().Number;
            }
        }

        private static async Task CallAdderSmartContract(IElrondProvider provider, Constants constants, Wallet wallet,
            AddressValue scAddress)
        {
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

            addRequest.SetGasLimit(new GasLimit(60000000));
            var addRequestTransaction = await addRequest.Send(wallet, provider);
            await addRequestTransaction.WaitForExecution(provider);
            addRequestTransaction.EnsureTransactionSuccess();

            // Query VM
            var result = await provider.QueryVm(new QueryVmRequestDto
            {
                FuncName = "getSum",
                ScAddress = scAddress.Bech32
            });

            var sumBytes = Convert.FromBase64String(result.Data.Data.ReturnData[0]);
            var sumHex = Convert.ToHexString(sumBytes);
        }

        private static async Task<AddressValue> DeploySmartContract(
            IElrondProvider provider,
            Constants constants,
            Wallet wallet,
            string filePath)
        {
            var account = wallet.GetAccount();
            await account.Sync(provider);

            var wasmFile = await Code.FromFilePath(filePath);
            var deployRequest = SmartContract.CreateDeploySmartContractTransactionRequest(
                constants,
                account,
                wasmFile,
                new CodeMetadata(false, true, false),
                NumericValue.BigIntValue(5));

            deployRequest.SetGasLimit(new GasLimit(60000000));

            // Get the deployed smart contract address based on account address and transaction nonce
            var smartContractAddress = SmartContract.ComputeAddress(account.Address, account.Nonce);
            var deployTransaction = await deployRequest.Send(wallet, provider);

            await deployTransaction.WaitForExecution(provider);
            deployTransaction.EnsureTransactionSuccess();

            return smartContractAddress;
        }
    }
}