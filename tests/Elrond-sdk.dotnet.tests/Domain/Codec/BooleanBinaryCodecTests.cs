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
            var actual = _sut.DecodeNested(buffer);

            // Assert
            Assert.AreEqual(true, actual.Value.ValueOf());
        }

        [Test]
        public void BooleanBinaryCodec_DecodeNested_False()
        {
            // Arrange
            var buffer = new byte[] {0x00};

            // Act
            var actual = _sut.DecodeNested(buffer);

            // Assert
            Assert.AreEqual(false, actual.Value.ValueOf());
        }
    }
}