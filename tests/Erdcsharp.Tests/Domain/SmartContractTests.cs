using Erdcsharp.Domain;
using Erdcsharp.Domain.SmartContracts;
using NUnit.Framework;

namespace Erdcsharp.UnitTests.Domain
{
    [TestFixture(Category = "UnitTests")]
    public class SmartContractTests
    {
        [Test]
        public void Should_ComputeAddress_Based_On_Account_And_Nonce()
        {
            // Arrange
            var        accountAddress = Address.FromBech32("erd1lkeja8knfjhkqzvrf3d9hmefxzt75wtf3vlg9m7ccugc8jmnrdpqy7yjeq");
            const long accountNonce   = 5;

            // Act
            var smartContractAddress = SmartContract.ComputeAddress(accountAddress, accountNonce);

            // Assert
            Assert.AreEqual("erd1qqqqqqqqqqqqqpgqfflfuh2wd78r4xgcusl4xtmj354aem0nrdpqc68c4h", smartContractAddress.Bech32);
        }
    }
}
