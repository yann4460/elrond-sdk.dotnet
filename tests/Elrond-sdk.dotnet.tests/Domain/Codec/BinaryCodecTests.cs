using Elrond.Dotnet.Sdk.Domain.Codec;
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
        public void BooleanBinaryCodec_DecodeNested_True()
        {
            // Arrange
            var buffer = new byte[] {0x01};
            var codec = new BinaryCoder();

            // Act
            var actual = codec.DecodeNested(buffer, TypeValue.Boolean);

            // Assert
            Assert.AreEqual(true, (actual.Value.ValueOf() as BooleanValue).IsTrue());
        }
    }
}