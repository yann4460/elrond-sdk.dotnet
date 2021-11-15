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
    public class AccountIntegrationTests
    {
        private IElrondProvider _provider;

        [SetUp]
        public void Setup()
        {
            _provider = new ElrondProvider(new HttpClient(), new ElrondNetworkConfiguration(Network.TestNet));
        }

        [Test(Description = "Synchronize an account from the network")]
        public async Task Should_Get_Alice_Balance()
        {
            var alice = new Account(Address.FromBech32(TestData.AliceBech32));
            await alice.Sync(_provider);

            Assert.That(alice.Balance.Token.Ticker, Is.EqualTo("EGLD"));
            Assert.That(alice.Nonce, Is.Not.Null);
            Assert.That(alice.Balance.Value, Is.Not.Null);
            Assert.That(alice.Balance.ToCurrencyString(), Is.Not.Null);
        }
    }
}
