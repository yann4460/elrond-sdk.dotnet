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

            await DeployAdderSmartContractAndQuery(provider, constants, wallet);
            //await QuerySmartContractWithoutAbi(provider, wallet);
            //await QuerySmartContractWithAbi(provider);
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

        private static async Task QuerySmartContractWithoutAbi(IElrondProvider provider, Constants constants,
            Wallet wallet)
        {
            var account = wallet.GetAccount();
            await account.Sync(provider);
            var scAddress = AddressValue.FromBech32("erd1qqqqqqqqqqqqqpgqjc4rtxq4q7ap37ujrud855ydy6rkslu5rdpqsum6wy");

            var queryTransaction = SmartContract.CreateCallSmartContractTransactionRequest(constants, account,
                scAddress, "getFullAuctionData",
                Balance.Zero(),
                new IBinaryType[]
                {
                    TokenIdentifierValue.From("TSTKR-209ea0"),
                    NumericValue.U64Value(3),
                });
            queryTransaction.SetGasLimit(new GasLimit(60000000));
            var transaction = await queryTransaction.Send(wallet, provider);
            await WaitForTransactionExecution("getFullAuctionData", transaction, provider);

            // Arrange
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
            var scResult = transaction.GetSmartContractResult(new[] {auction});
        }

        private static async Task QuerySmartContractWithAbi(IElrondProvider provider)
        {
            var fileBytes = await File.ReadAllBytesAsync("SmartContracts/auction/auction.abi.json");
            var json = Encoding.UTF8.GetString(fileBytes);
            var jsonSerializerOptions = new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
            var abiDefinition = JsonSerializer.Deserialize<AbiDefinition>(json, jsonSerializerOptions);
            var response = await SmartContract.QuerySmartContract(
                AddressValue.FromBech32("erd1qqqqqqqqqqqqqpgqjc4rtxq4q7ap37ujrud855ydy6rkslu5rdpqsum6wy"),
                "getFullAuctionData",
                new IBinaryType[]
                {
                    TokenIdentifierValue.From("TSTKR-209ea0"),
                    NumericValue.U64Value(3),
                },
                abiDefinition, provider);
        }

        private static async Task DeployAdderSmartContractAndQuery(IElrondProvider provider,
            Constants constants,
            Wallet wallet)
        {
            var fileBytes = await File.ReadAllBytesAsync("SmartContracts/adder/adder.abi.json");
            var json = Encoding.UTF8.GetString(fileBytes);
            var jsonSerializerOptions = new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
            var abi = JsonSerializer.Deserialize<AbiDefinition>(json, jsonSerializerOptions);

            var account = wallet.GetAccount();

            var smartContractAddress =
                await DeploySmartContract(provider, constants, wallet, account, "SmartContracts/adder/adder.wasm");

            //2. Call 'add' method
            var addRequest = SmartContract.CreateCallSmartContractTransactionRequest(constants, account,
                smartContractAddress, "add", Balance.Zero(),
                new IBinaryType[]
                {
                    NumericValue.BigIntValue(12)
                });
            addRequest.SetGasLimit(new GasLimit(60000000));
            var addRequestTransaction = await addRequest.Send(wallet, provider);
            await WaitForTransactionExecution("Add", addRequestTransaction, provider);

            //3. Query VM
            var result = await provider.QueryVm(new QueryVmRequestDto
            {
                FuncName = "getSum",
                ScAddress = smartContractAddress.Bech32
            });

            var sumBytes = Convert.FromBase64String(result.Data.Data.ReturnData[0]);
            var sumHex = Convert.ToHexString(sumBytes);


            //4. Ex query smart contract 

            var getSum = SmartContract.CreateCallSmartContractTransactionRequest(constants, account,
                smartContractAddress, "getSum", Balance.Zero(), new IBinaryType[0]);

            getSum.SetGasLimit(new GasLimit(60000000));
            var getSumTransaction = await getSum.Send(wallet, provider);
            await WaitForTransactionExecution("getSum", getSumTransaction, provider);

            var getSumResult = getSumTransaction.GetSmartContractResult("getSum", abi);
            var sum = getSumResult[0].ValueOf<NumericValue>().Number;
            Debug.Assert(sum.ToString().Equals("17"));
        }

        private static async Task<AddressValue> DeploySmartContract(
            IElrondProvider provider,
            Constants constants,
            Wallet wallet,
            Account account,
            string filePath)
        {
            await account.Sync(provider);

            //1. Deploy smart contract from wasm
            var wasmFile = await File.ReadAllBytesAsync(filePath);
            var deployRequest = SmartContract.CreateDeploySmartContractTransactionRequest(constants, account,
                new Code(wasmFile),
                new CodeMetadata(false, true, false),
                new IBinaryType[]
                {
                    NumericValue.BigIntValue(5)
                });

            deployRequest.SetGasLimit(new GasLimit(60000000));

            var deployTransaction = await deployRequest.Send(wallet, provider);
            await WaitForTransactionExecution("DeploySmartContract", deployTransaction, provider);
            var smartContractAddress = SmartContract.ComputeAddress(account.Address, account.Nonce - 1);
            return smartContractAddress;
        }

        private static async Task WaitForTransactionExecution(string transactionName, Transaction transaction,
            IElrondProvider provider)
        {
            System.Console.WriteLine($"Send : {transactionName}");
            do
            {
                await Task.Delay(1000);
                await transaction.Sync(provider);
                System.Console.WriteLine($" - Status : {transaction.Status}");
            } while (transaction.IsPending());

            if (!transaction.IsSuccessful())
                throw new Exception("Invalid transaction : {transactionName}");
        }
    }
}