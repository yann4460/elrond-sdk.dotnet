using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Elrond.Dotnet.Sdk.Domain;
using Elrond.Dotnet.Sdk.Provider;

namespace Elrond.SDK.Console
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var mnemonicBob =
                "corn maximum tunnel inhale urge lazy language trash balance artwork pyramid width unable amused style water royal absorb vessel photo creek tent picnic mercy";
            var mnemonicAlice =
                "maximum tunnel inhale corn urge lazy language trash balance artwork pyramid width unable amused style water royal absorb vessel photo creek tent picnic mercy";


            var bob = CreateNewAccount(mnemonicBob, "bob");
            var alice = CreateNewAccount(mnemonicAlice, "alice");

            Debug.Assert(bob.Bech32 == "erd15eaep0g7y9r5543ucfwsvnzjf05ld4l240txcj365qk2q4rrrqlq7qnsrp");
            Debug.Assert(alice.Bech32 == "erd1y8lulq84pf2snsmjwuudx46pukky76rey7kepxxul6e5x3rwlqns3nhr89");

            var provider = new ElrondProvider(new HttpClient {BaseAddress = new Uri("https://testnet-api.elrond.com")});
            var constants = await Constants.GetFromNetwork(provider);

            var accountBob = new Account(Address.FromBech32(bob.Bech32));
            var walletBob = Wallet.DeriveFromKeyFile(bob, "bob");

            await accountBob.Sync(provider);

            var accountAlice = new Account(Address.FromHex(alice.Address));
            var walletAlice = Wallet.DeriveFromKeyFile(alice, "alice");
            await accountAlice.Sync(provider);

            var smartContractAddress = Address.FromBech32("erd1qqqqqqqqqqqqqpgq68cl28dccd6th6fcajej8q8ru6gpmhcqlqnscht36c"); //Belong to alice
            var mintRequest = SmartContract.CreateCallSmartContractTransactionRequest(constants, accountAlice,
                smartContractAddress, "mint", Balance.Zero(), new[]
                {
                    Argument.CreateArgumentFromInt64(1),
                    Argument.CreateArgumentFromHex(bob.Address)
                });

            await mintRequest.ComputeGasLimit(provider);
            var mintTransaction = await mintRequest.Send(walletAlice, provider);
            await WaitForTransactionExecution("mint", mintTransaction, provider);


            var totalMintedRequest = SmartContract.CreateCallSmartContractTransactionRequest(constants, accountBob,
                smartContractAddress, "totalMinted", Balance.Zero());
            await totalMintedRequest.ComputeGasLimit(provider);
            var totalMintedTransaction = await totalMintedRequest.Send(walletBob, provider);
            await WaitForTransactionExecution("totalMinted", totalMintedTransaction, provider);

            var totalMinted = totalMintedTransaction.GetSmartContractResult<long>(1);

            var lastTokenId = totalMinted - 1;

            var tokenOwnerRequest = SmartContract.CreateCallSmartContractTransactionRequest(constants, accountBob,
                smartContractAddress, "tokenOwner", Balance.Zero(),
                new[] {Argument.CreateArgumentFromInt64(lastTokenId)});
            await tokenOwnerRequest.ComputeGasLimit(provider);
            var tokenOwnerTransaction = await tokenOwnerRequest.Send(walletBob, provider);

            await WaitForTransactionExecution("tokenOwner", tokenOwnerTransaction, provider);

            var tokenOwner = tokenOwnerTransaction.GetSmartContractResult<Address>(1);
            System.Console.WriteLine($"Owner of token {lastTokenId} is {tokenOwner}");
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
            System.Console.WriteLine(
                $" - SmartContractResponseStatus : {transaction.GetSmartContractResult<string>()}");
            System.Console.WriteLine("*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-");
        }

        private static KeyFile CreateNewAccount(string mnemonic, string password)
        {
            var wallet = Wallet.DeriveFromMnemonic(mnemonic);
            return wallet.BuildKeyFile(password);
        }
    }
}