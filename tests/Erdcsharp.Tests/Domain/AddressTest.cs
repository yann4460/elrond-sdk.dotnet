using System;
using Erdcsharp.Domain;
using Erdcsharp.Domain.Exceptions;
using Erdcsharp.UnitTests.FakeData;
using NUnit.Framework;

namespace Erdcsharp.UnitTests.Domain
{
    [TestFixture(Category = "UnitTests")]
    public class AddressTest
    {
        [Test]
        public void FromBech32_Should_Create_Address()
        {
            // Act
            var address = Address.FromBech32(TestData.AliceBech32);

            // Assert
            Assert.That(address.Hex.Equals(TestData.AliceHex, StringComparison.CurrentCultureIgnoreCase));
            Assert.That(address.Bech32.Equals(TestData.AliceBech32, StringComparison.CurrentCultureIgnoreCase));
        }

        [Test]
        public void FromHex_Should_Create_Address()
        {
            // Act
            var address = Address.FromHex(TestData.AliceHex);

            // Assert
            Assert.That(address.Hex.Equals(TestData.AliceHex, StringComparison.CurrentCultureIgnoreCase));
            Assert.That(address.Bech32.Equals(TestData.AliceBech32, StringComparison.CurrentCultureIgnoreCase));
        }

        [Test]
        public void EqualTo_Should_Test_Equality()
        {
            // Arrange
            var aliceFoo = Address.FromHex(TestData.AliceHex);
            var aliceBar = Address.FromBech32(TestData.AliceBech32);
            var bob      = Address.FromHex(TestData.BobHex);

            // Assert
            Assert.That(aliceBar, Is.EqualTo(aliceFoo));
            Assert.That(aliceFoo, Is.EqualTo(aliceBar));
            Assert.That(bob, Is.Not.EqualTo(null));
            Assert.That(bob, Is.Not.EqualTo(aliceFoo));
            Assert.That(bob, Is.Not.EqualTo(aliceBar));
        }

        [TestCase("erd1qyu5wthldzr8wx5c9ucg8kjagg0jfs53s8nr3zpz3hypefsdd8ssycr6th", false)]
        [TestCase("erd1qqqqqqqqqqqqqqqpqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqzllls8a5w6u", true)]
        public void IsContractAddress_Should_Return_ExpectedResult(string address, bool expectedResult)
        {
            // Arrange
            var scAddress = Address.FromBech32(address);

            // Act
            var isSmartContract = scAddress.IsContractAddress();

            // Assert
            Assert.That(isSmartContract, Is.EqualTo(expectedResult));
        }

        [TestCase("foo")]
        [TestCase("aaaaaaa")]
        [TestCase("0D65416545")]
        [TestCase("erd1l453hd0gt5gzdp7czpuall8ggt2dcv5zwmfdf3sd3lguxseux2")]
        [TestCase("xerd1l453hd0gt5gzdp7czpuall8ggt2dcv5zwmfdf3sd3lguxseux2fsmsgldz")]
        public void From_Should_Throw_Error_When_Invalid_Input(string input)
        {
            Assert.Throws<CannotCreateAddressException>(() => Address.From(input));
        }
    }
}
