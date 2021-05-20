using Elrond.Dotnet.Sdk.Domain.Codec;
using NUnit.Framework;

namespace Elrond_sdk.dotnet.tests.Domain.Codec
{
    public class BooleanBinaryCodecTests
    {
        private BooleanBinaryCodec _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new BooleanBinaryCodec();
        }

        [Test]
        public void BooleanBinaryCodec_DecodeNested_True()
        {
            // Arrange
            var buffer = new byte[] {0x01};

            // Act
            var actual = _sut.DecodeNested(buffer, TypeValue.Boolean);

            // Assert
            Assert.IsTrue((actual.Value.ValueOf() as BooleanValue).IsTrue());
        }

        [Test]
        public void BooleanBinaryCodec_DecodeNested_False()
        {
            // Arrange
            var buffer = new byte[] {0x00};

            // Act
            var actual = _sut.DecodeNested(buffer, TypeValue.Boolean);

            // Assert
            Assert.IsTrue((actual.Value.ValueOf() as BooleanValue).IsFalse());
        }
    }
}