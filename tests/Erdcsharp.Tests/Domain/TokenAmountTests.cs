using System.Numerics;
using Erdcsharp.Domain;
using NUnit.Framework;

namespace Erdcsharp.UnitTests.Domain
{
    [TestFixture(Category = "UnitTests")]
    public class TokenAmountTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TokenAmount_Zero_Should_Return_ZeroValue()
        {
            // Act
            var zero = TokenAmount.Zero();

            // Assert
            Assert.That(zero.Value.IsZero, Is.True);
        }

        [Test]
        public void TokenAmount_From_Should_Return_FullValue()
        {
            // Act
            var tokenAmount = TokenAmount.From("1000000000000000000");

            // Assert
            Assert.That(tokenAmount.Value, Is.EqualTo(BigInteger.Parse("1000000000000000000")));
        }

        [Test]
        public void TokenAmount_EGLD_Should_CreateValueFromEGLD()
        {
            // Act
            var oneEgld = TokenAmount.EGLD("1");

            // Assert
            Assert.That(oneEgld.Value, Is.EqualTo(BigInteger.Parse("1000000000000000000")));
        }

        [Test]
        public void TokenAmount_ToCurrencyString_Should_FormatAmount()
        {
            // Arrange
            var oneEgld = TokenAmount.EGLD("1");

            // Act
            var stringValue = oneEgld.ToCurrencyString();

            // Assert
            Assert.That(stringValue, Is.EqualTo("1.000000000000000000 EGLD"));
        }

        [Test]
        public void TokenAmount_EGLD_Should_GetCorrectAmount()
        {
            // Act
            var egld = TokenAmount.EGLD("0.01");

            // Assert
            Assert.That(egld.ToString(), Is.EqualTo("10000000000000000"));
        }

        [Test]
        public void TokenAmount_EGLD_Should_GetBillionAmount()
        {
            // Act
            var egld = TokenAmount.EGLD("1000000000");

            // Assert
            Assert.That(egld.ToString(), Is.EqualTo("1000000000000000000000000000"));
        }

        [Test]
        public void TokenAmount_ToCurrencyString_WithCustomToken_Should_FormatAmount()
        {
            // Arrange
            var amount = TokenAmount.ESDT("1000000000", Token.ESDT("MyToken", "MTK", 7));

            // Act
            var stringValue = amount.ToCurrencyString();

            // Assert
            Assert.That(stringValue, Is.EqualTo("1000000000.0000000 MTK"));
        }

        [Test]
        public void TokenAmount_ToCurrencyString_WithCustomToken_Should_FormatAmount_From()
        {
            // Arrange
            var amount = TokenAmount.From("10000000000000000", Token.ESDT("MyToken", "MTK", 7));

            // Act
            var stringValue = amount.ToCurrencyString();

            // Assert
            Assert.That(stringValue, Is.EqualTo("1000000000.0000000 MTK"));
        }
    }
}
