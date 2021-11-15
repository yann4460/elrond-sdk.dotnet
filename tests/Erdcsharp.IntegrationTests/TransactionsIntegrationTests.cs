using System;
using System.Net.Http;
using System.Threading.Tasks;
using Erdcsharp.Configuration;
using Erdcsharp.Domain;
using Erdcsharp.Provider;
using Erdcsharp.UnitTests.FakeData;
using NUnit.Framework;

namespace Erdcsharp.IntegrationTests
{
    [TestFixture(Category = "LongRunning", Description = "Transaction interaction usage")]
    public class TransactionsIntegrationTests
    {
        private IElrondProvider   _provider;
        private TestWalletFactory _testWalletFactory;

        [SetUp]
        public void Setup()
        {
            _provider          = new ElrondProvider(new HttpClient(), new ElrondNetworkConfiguration(Network.TestNet));
            _testWalletFactory = new TestWalletFactory();
        }

        [Test(Description = "Send a 4 EGLD transaction from alice to bob then transfer back the money")]
        public async Task Should_Send_Transactions()
        {
            var networkConfig = await NetworkConfig.GetFromNetwork(_provider);
            var alice         = _testWalletFactory.Alice.GetAccount();
            var bob           = _testWalletFactory.Bob.GetAccount();

            await alice.Sync(_provider);
            await bob.Sync(_provider);
            var initialBalanceOfBob = bob.Balance;

            // Alice send 4 EGLD to Bob
            var txRequestOne   = TransactionRequest.Create(alice, networkConfig, bob.Address, TokenAmount.EGLD("4"));
            var transactionOne = await txRequestOne.Send(_provider, _testWalletFactory.Alice);
            await transactionOne.AwaitExecuted(_provider);

            await alice.Sync(_provider);
            await bob.Sync(_provider);
            var initialBalanceOfAlice = alice.Balance;
            var newBalanceOfBob       = bob.Balance;

            // Bob send 4 EGLD back to Bob
            var txRequestTwo   = TransactionRequest.Create(bob, networkConfig, alice.Address, TokenAmount.EGLD("4"));
            var transactionTwo = await txRequestTwo.Send(_provider, _testWalletFactory.Bob);
            await transactionTwo.AwaitExecuted(_provider);

            await alice.Sync(_provider);
            var newBalanceOfAlice = alice.Balance;

            Assert.That(newBalanceOfBob.Value, Is.EqualTo(initialBalanceOfBob.Value + TokenAmount.EGLD("4").Value));
            Assert.That(newBalanceOfAlice.Value,
                        Is.EqualTo(initialBalanceOfAlice.Value + TokenAmount.EGLD("4").Value));
        }

        [Test(Description = "Send a 1 EGLD transaction from alice to alice await await")]
        public async Task Should_Await_Transaction_Is_Notarized()
        {
            var networkConfig = await NetworkConfig.GetFromNetwork(_provider);
            var alice         = _testWalletFactory.Alice.GetAccount();

            await alice.Sync(_provider);

            // Alice send 1 EGLD to alice
            var txRequest = TransactionRequest.Create(alice, networkConfig, alice.Address, TokenAmount.EGLD("1"));
            var tx        = await txRequest.Send(_provider, _testWalletFactory.Alice);
            await tx.AwaitNotarized(_provider);

            Assert.Pass();
        }
    }
}
