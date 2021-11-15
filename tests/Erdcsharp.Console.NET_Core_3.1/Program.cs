using System;
using System.Net.Http;
using System.Threading.Tasks;
using Erdcsharp.Configuration;
using Erdcsharp.Domain;
using Erdcsharp.Provider;

namespace Erdcsharp.Console.NET_Core_3._1
{
    public static class Program
    {
        private const string AliceSecretHex = "413f42575f7f26fad3317a778771212fdb80245850981e48b58a4f25e344e8f9";
        private const string BobBech32      = "erd1spyavw0956vq68xj8y4tenjpq2wd5a9p2c6j8gsz7ztyrnpxrruqzu66jx";

        public static async Task Main(string[] args)
        {
            var provider  = new ElrondProvider(new HttpClient(), new ElrondNetworkConfiguration(Network.TestNet));
            var wallet    = new Wallet(AliceSecretHex);
            var constants = await NetworkConfig.GetFromNetwork(provider);

            await SynchronizingNetworkParameter(provider);
            await SynchronizingAnAccountObject(provider, wallet.GetAccount());

            await CreatingValueTransferTransactions(provider, constants, wallet);
        }

        private static async Task SynchronizingNetworkParameter(IElrondProvider provider)
        {
            System.Console.WriteLine("SynchronizingNetworkParameter");

            var constants = await NetworkConfig.GetFromNetwork(provider);

            System.Console.WriteLine("MinGasPrice {0}", constants.MinGasPrice);
            System.Console.WriteLine("ChainId {0}", constants.ChainId);
            System.Console.WriteLine("GasPerDataByte {0}", constants.GasPerDataByte);
            System.Console.WriteLine("-*-*-*-*-*" + Environment.NewLine);
        }

        private static async Task SynchronizingAnAccountObject(IElrondProvider provider, Account account)
        {
            System.Console.WriteLine("SynchronizingAnAccountObject");

            await account.Sync(provider);

            System.Console.WriteLine("Balance {0}", account.Balance);
            System.Console.WriteLine("Nonce {0}", account.Nonce);

            System.Console.WriteLine("-*-*-*-*-*" + Environment.NewLine);
        }

        private static async Task CreatingValueTransferTransactions(IElrondProvider provider, NetworkConfig networkConfig,
                                                                    Wallet wallet)
        {
            System.Console.WriteLine("CreatingValueTransferTransactions");

            var sender   = wallet.GetAccount();
            var receiver = Address.FromBech32(BobBech32);
            await sender.Sync(provider);

            var transaction = TransactionRequest.Create(sender, networkConfig, receiver, TokenAmount.EGLD("0.000000000000054715"));
            transaction.SetData("Hello world !");
            transaction.SetGasLimit(GasLimit.ForTransfer(networkConfig, transaction));

            var transactionResult = await transaction.Send(provider, wallet);
            await transactionResult.AwaitExecuted(provider);
            transactionResult.EnsureTransactionSuccess();

            System.Console.WriteLine("TxHash {0}", transactionResult.TxHash);
            System.Console.WriteLine("-*-*-*-*-*" + Environment.NewLine);
        }
    }
}
