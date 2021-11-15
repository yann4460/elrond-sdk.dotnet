using Erdcsharp.Domain;
using NUnit.Framework;

namespace Erdcsharp.UnitTests.Domain
{
    [TestFixture(Category = "UnitTests")]
    public class TokenTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Token_EGLD_Should_Have_18_DecimalPrecision()
        {
            // Act
            var egldToken = Token.EGLD();

            // Assert
            Assert.That(egldToken.DecimalPrecision, Is.EqualTo(18));
            Assert.That(egldToken.Ticker, Is.EqualTo(Constants.EGLD));
            Assert.That(egldToken.One().ToString(), Is.EqualTo("1000000000000000000"));
        }

        [Test]
        public void Token_ESDT_One_Should_Return_Correct_Decimal_Precision()
        {
            // Arrange
            var token = Token.ESDT("MyCustomToken", "MCTK", 10);

            // Act
            var one = token.One();

            // Assert
            Assert.That(one.ToString(), Is.EqualTo("10000000000"));
        }

        [Test]
        public void Token_ESDT_Zero_Should_Return_0()
        {
            // Arrange
            var token = Token.ESDT("MyCustomToken", "MCTK", 10);

            // Act
            var zero = token.Zero();

            // Assert
            Assert.That(zero.IsZero, Is.True);
        }
    }
}
