using Erdcsharp.Domain.Codec;
using Erdcsharp.Domain.Values;
using NUnit.Framework;

namespace Erdcsharp.UnitTests.Domain.Codec
{
    [TestFixture(Category = "UnitTests")]
    public class BooleanBinaryCodecTests
    {
        private BooleanBinaryCodec _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new BooleanBinaryCodec();
        }

        [Test]
        public void DecodeNested_True()
        {
            // Arrange
            var buffer = new byte[] {0x01};

            // Act
            var actual = _sut.DecodeNested(buffer, TypeValue.BooleanValue);

            // Assert
            Assert.IsTrue(actual.Value.ValueOf<BooleanValue>().IsTrue());
        }

        [Test]
        public void DecodeNested_False()
        {
            // Arrange
            var buffer = new byte[] {0x00};

            // Act
            var actual = _sut.DecodeNested(buffer, TypeValue.BooleanValue);

            // Assert
            Assert.IsTrue(actual.Value.ValueOf<BooleanValue>().IsFalse());
        }


        [Test]
        public void EncodeNested_True()
        {
            // Arrange
            var booleanValue = BooleanValue.From(true);

            // Act
            var actual = _sut.EncodeNested(booleanValue);

            // Assert
            Assert.That(actual.Length, Is.EqualTo(1));
            Assert.That(actual[0], Is.EqualTo(0x01));
        }

        [Test]
        public void EncodeNested_False()
        {
            // Arrange
            var booleanValue = BooleanValue.From(false);

            // Act
            var actual = _sut.EncodeNested(booleanValue);

            // Assert
            Assert.That(actual.Length, Is.EqualTo(1));
            Assert.That(actual[0], Is.EqualTo(0x00));
        }


        [Test]
        public void DecodeTop_True()
        {
            // Arrange
            var buffer = new byte[] {0x01};

            // Act
            var actual = _sut.DecodeTopLevel(buffer, TypeValue.BooleanValue);

            // Assert
            Assert.IsTrue(actual.ValueOf<BooleanValue>().IsTrue());
        }

        [Test]
        public void DecodeTop_False()
        {
            // Arrange
            var buffer = new byte[] {0x00};

            // Act
            var actual = _sut.DecodeTopLevel(buffer, TypeValue.BooleanValue);

            // Assert
            Assert.IsTrue(actual.ValueOf<BooleanValue>().IsFalse());
        }


        [Test]
        public void Encode_True()
        {
            // Arrange
            var booleanValue = BooleanValue.From(true);

            // Act
            var actual = _sut.EncodeTopLevel(booleanValue);

            // Assert
            Assert.That(actual.Length, Is.EqualTo(1));
            Assert.That(actual[0], Is.EqualTo(0x01));
        }

        [Test]
        public void Encode_False()
        {
            // Arrange
            var booleanValue = BooleanValue.From(false);

            // Act
            var actual = _sut.EncodeTopLevel(booleanValue);

            // Assert
            Assert.That(actual.Length, Is.EqualTo(0));
        }
    }
}
