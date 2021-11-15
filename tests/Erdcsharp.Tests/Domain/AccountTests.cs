using System.Threading.Tasks;
using Erdcsharp.Domain;
using Erdcsharp.Provider;
using Erdcsharp.Provider.Dtos;
using Erdcsharp.UnitTests.FakeData;
using Moq;
using NUnit.Framework;

namespace Erdcsharp.UnitTests.Domain
{
    [TestFixture(Category = "UnitTests")]
    public class AccountTests
    {
        private Mock<IElrondProvider> _mockProvider;

        [SetUp]
        public void Setup()
        {
            _mockProvider = new Mock<IElrondProvider>();
        }

        [Test]
        public void Account_Should_Build_With_Address()
        {
            // Arrange
            var aliceAddress = Address.From(TestData.AliceHex);

            // Act
            var account = new Account(aliceAddress);

            // Assert
            Assert.That(account.Address, Is.EqualTo(aliceAddress));
            Assert.That(account.Nonce, Is.EqualTo(0));
        }

        [Test]
        public async Task Sync_Should_Synchronize_Account_With_Network()
        {
            // Arrange
            var account = new Account(Address.From(TestData.AliceHex));
            var address = account.Address.Bech32;
            _mockProvider.Setup(p => p.GetAccount(It.IsAny<string>())).ReturnsAsync(new AccountDto
            {
                Address  = address,
                Balance  = "99882470417129999997",
                Nonce    = 2555546,
                Username = "elrond"
            });

            // Act
            await account.Sync(_mockProvider.Object);

            // Assert
            _mockProvider.Verify(s => s.GetAccount(address), Times.Once);
            Assert.That(account.Address.Bech32, Is.EqualTo(address));
            Assert.That(account.Nonce, Is.EqualTo(2555546));
            Assert.That(account.Balance.ToDenominated(), Is.EqualTo("99.882470417129999997"));
        }
    }
}
