using Elrond.Dotnet.Sdk.Domain.Codec;
using Elrond.Dotnet.Sdk.Domain.Values;
using NUnit.Framework;

namespace Elrond_sdk.dotnet.tests.Domain.Codec
{
    public class BinaryCodecTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void DecodeNested_False()
        {
            // Arrange
            var buffer = new byte[] { 0x01 };
            var codec = new BinaryCodec();

            // Act
            var actual = codec.DecodeNested(buffer, TypeValue.BooleanValue);

            BytesValue.FromBuffer(new byte[] { 0x00 });
            // Assert
            Assert.AreEqual(true, (actual.Value.ValueOf<BooleanValue>()).IsTrue());
        }

        [Test]
        public void DecodeNested_True()
        {
            // Arrange
            var buffer = new byte[] { 0x01 };
            var codec = new BinaryCodec();

            // Act
            var actual = codec.DecodeNested(buffer, TypeValue.BooleanValue);

            BytesValue.FromBuffer(new byte[] { 0x00 });
            // Assert
            Assert.AreEqual(true, (actual.Value.ValueOf<BooleanValue>()).IsTrue());
        }
    }
}