using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Elrond.Dotnet.Sdk.Domain;
using Elrond.Dotnet.Sdk.Provider;
using Elrond.Dotnet.Sdk.Provider.Dtos;

namespace Elrond.SDK.Console
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateToken();
        }

        private static async Task CreateToken()
        {
            var privateKey =
                "C5A89BFA5E8FFFA4BAA732D8D8EE9503FAFA538599C3DDEE28D21F64DFFDBF00FDB32E9ED34CAF6009834C5A5BEF293097EA39698B3E82EFD8C71183CB731B42";
            var wallet = new Wallet(privateKey);
            var kf = wallet.BuildKeyFile(string.Empty);
            var account = new Account(Address.FromBech32(kf.Bech32));

            var client = new HttpClient {BaseAddress = new Uri("https://testnet-gateway.elrond.com")};
            var provider = new ElrondProvider(client);
            var constants = await Constants.GetFromNetwork(provider);

            await account.Sync(provider);

            //1. Deploy smart contract from wasm
            var wasmFile = await File.ReadAllBytesAsync("SmartContracts/adder/adder.wasm");
            var deployRequest = SmartContract.CreateDeploySmartContractTransactionRequest(constants, account,
                new Code(wasmFile),
                new CodeMetadata(false, true, false),
                new[]
                {
                    Argument.CreateArgumentFromInt64(5)
                });
            deployRequest.SetGasLimit(new GasLimit(60000000));

            var deployTransaction = await deployRequest.Send(wallet, provider);
            var smartContractAddress = SmartContract.ComputeAddress(account.Address, account.Nonce - 1);
            await WaitForTransactionExecution("Deployment", deployTransaction, provider);

            //2. Call 'add' method
            var addRequest = SmartContract.CreateCallSmartContractTransactionRequest(constants, account,
                smartContractAddress, "add", Balance.Zero(),
                new[]
                {
                    Argument.CreateArgumentFromInt64(12)
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
            var sum = Argument.GetValue<long>(sumHex, 0); //Should be 17 !
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

            System.Console.WriteLine("*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-");
        }
    }
}