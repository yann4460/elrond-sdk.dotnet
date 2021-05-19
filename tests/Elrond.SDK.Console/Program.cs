using System;
using System.IO;
using System.Net.Http;
using System.Text;
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
            var testPrivateKey =
                "C5A89BFA5E8FFFA4BAA732D8D8EE9503FAFA538599C3DDEE28D21F64DFFDBF00FDB32E9ED34CAF6009834C5A5BEF293097EA39698B3E82EFD8C71183CB731B42";

            //ESDTNFTTransfer@5453544b522d323039656130@02@01@00000000000000000500962a35981507ba18fb921f1a7a508d2687687f941b42@61756374696f6e546f6b656e@016345785d8a0000@008ac7230489e80000@60a443a1@@
            //0000000445474c44000000000000000000000008016345785d8a0000000000088ac7230489e800000000000060a44594fdb32e9ed34caf6009834c5a5bef293097ea39698b3e82efd8c71183cb731b420000000000000000000000000000000000000000000000000000000000000000000000000000000201f400000000

            //00000004
            //45474c44                                                                      //payment_token
            //0000000000000000
            //00000008                                                                       //
            //016345785d8a0000                                                              //min_bid 
            //00000008                                                                      //
            //8ac7230489e80000                                                              //max_bid
            //00000000                                                                      //
            //60a44594                                                                      //deadline
            //fdb32e9ed34caf6009834c5a5bef293097ea39698b3e82efd8c71183cb731b42              //original_owner
            //00000000000000000000000000000000000000000000000000000000000000000000000000000002
            //01f                                                                           //marketplace_cut_percentage
            //400000000

            var wallet = new Wallet(testPrivateKey);
            var client = new HttpClient {BaseAddress = new Uri("https://testnet-gateway.elrond.com")};
            var provider = new ElrondProvider(client);

            var query = new QueryVmRequestDto()
            {
                ScAddress = "erd1qqqqqqqqqqqqqpgqjc4rtxq4q7ap37ujrud855ydy6rkslu5rdpqsum6wy",
                FuncName = "getFullAuctionData",
                Args = new[]
                {
                    Argument.CreateArgumentFromUtf8String("TSTKR-209ea0").Value,
                    Argument.CreateArgumentFromInt64(3).Value,
                }
            };
            var result = await provider.QueryVm(query);
          
            var data = Convert.FromBase64String("RVNEVE5GVFRyYW5zZmVyQDU0NTM1NDRiNTIyZDMyMzAzOTY1NjEzMEAwMkAwMUAwMDAwMDAwMDAwMDAwMDAwMDUwMDk2MmEzNTk4MTUwN2JhMThmYjkyMWYxYTdhNTA4ZDI2ODc2ODdmOTQxYjQyQDYxNzU2Mzc0Njk2ZjZlNTQ2ZjZiNjU2ZUAwMTYzNDU3ODVkOGEwMDAwQDAwOGFjNzIzMDQ4OWU4MDAwMEA2MGE0NDNhMUBA");
            var strinsazeg = Encoding.UTF8.GetString(data);


            var kf = wallet.BuildKeyFile(string.Empty);
            var account = new Account(Address.FromBech32(kf.Bech32));
            var constants = await Constants.GetFromNetwork(provider);
            await account.Sync(provider);

            var unixTime = (ulong)((DateTimeOffset)DateTime.Now.AddMinutes(1)).ToUnixTimeSeconds();
            var transaction = SmartContract.CreateCallSmartContractTransactionRequest(constants, account, account.Address, "ESDTNFTTransfer", Balance.Zero(),
                new[]
                {
                    Argument.CreateArgumentFromUtf8String("TSTKR-209ea0"),
                    Argument.CreateArgumentFromInt64(2), Argument.CreateArgumentFromInt64(1),
                    Argument.CreateArgumentFromAddress(Address.FromBech32("erd1qqqqqqqqqqqqqpgqjc4rtxq4q7ap37ujrud855ydy6rkslu5rdpqsum6wy")),
                    Argument.CreateArgumentFromUtf8String("auctionToken"),
                    Argument.CreateArgumentFromBalance(Balance.EGLD("0.1")),
                    Argument.CreateArgumentFromBalance(Balance.EGLD("10")), Argument.CreateArgumentFromUInt64(unixTime),
                    Argument.CreateArgumentFromUtf8String(string.Empty), Argument.CreateArgumentFromUtf8String(string.Empty)
                });

            transaction.SetGasLimit(new GasLimit(60000000));

            var aze = await transaction.Send(wallet, provider);


            //await DeployAdderSmartContractAndQuery(provider, wallet);
            //45474c44
            //000000000000000000000008016345785d8a0000000000088ac7230489e800000000000060a42a8dfdb32e9ed34caf6009834c5a5bef293097ea39698b3e82efd8c71183cb731b420000000000000000000000000000000000000000000000000000000000000000000000000000000201f400000000

        }

        private static async Task DeployAdderSmartContractAndQuery(IElrondProvider provider, Wallet wallet)
        {
            var constants = await Constants.GetFromNetwork(provider);
            var kf = wallet.BuildKeyFile(string.Empty);
            var account = new Account(Address.FromBech32(kf.Bech32));

            var smartContractAddress =
                await DeploySmartContract(provider, constants, wallet, account, "SmartContracts/adder/adder.wasm");

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

        private static async Task<Address> DeploySmartContract(
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
                new[]
                {
                    Argument.CreateArgumentFromInt64(5)
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