using Elrond.Dotnet.Sdk.Domain;
using Elrond.Dotnet.Sdk.Domain.Values;
using NUnit.Framework;

namespace Elrond_sdk.dotnet.tests.Domain
{
    public class SmartContractTests
    {
        [Test]
        public void Should_ComputeAddress_Based_On_Account_And_Nonce()
        {
            // Arrange
            var accountAddress = AddressValue.FromBech32("erd1lkeja8knfjhkqzvrf3d9hmefxzt75wtf3vlg9m7ccugc8jmnrdpqy7yjeq");
            const int accountNonce = 5;

            // Act
            var smartContractAddress = SmartContract.ComputeAddress(accountAddress, accountNonce);

            // Assert
            Assert.AreEqual("erd1qqqqqqqqqqqqqpgqfflfuh2wd78r4xgcusl4xtmj354aem0nrdpqc68c4h", smartContractAddress.Bech32);
        }
    }
}