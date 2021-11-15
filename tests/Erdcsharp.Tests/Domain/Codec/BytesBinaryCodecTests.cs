using System;
using Erdcsharp.Domain.Codec;
using Erdcsharp.Domain.Values;
using NUnit.Framework;

namespace Erdcsharp.UnitTests.Domain.Codec
{
    [TestFixture(Category = "UnitTests")]
    public class BytesBinaryCodecTests
    {
        private BytesBinaryCodec _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new BytesBinaryCodec();
        }

        [Test]
        public void BytesBinaryCodec_DecodeNested()
        {
            var buffer = Convert.FromHexString("0000000445474c44");

            // Act
            var actual = _sut.DecodeNested(buffer, TypeValue.BytesValue);

            var hex = Convert.ToHexString((actual.Value.ValueOf<BytesValue>()).Buffer);

            // Assert
            Assert.AreEqual("45474C44", hex);
        }

        [Test]
        public void BytesBinaryCodec_EncodeNested()
        {
            // Arrange
            var buffer = Convert.FromHexString("45474C44");

            var value = new BytesValue(buffer, TypeValue.BytesValue);

            // Act
            var actual = _sut.EncodeNested(value);
            var hex    = Convert.ToHexString(actual);

            // Assert
            Assert.AreEqual(buffer.Length + 4, actual.Length);
            Assert.AreEqual("0000000445474C44", hex);
        }

        [Test]
        public void BytesBinaryCodec_EncodeNested_AndDecode()
        {
            // Arrange
            var expected = "FDB32E9ED34CAF6009834C5A5BEF293097EA39698B3E82EFD8C71183CB731B42";
            var buffer   = Convert.FromHexString(expected);
            var value    = new BytesValue(buffer, TypeValue.BytesValue);

            // Act
            var encoded = _sut.EncodeNested(value);
            var actual  = _sut.DecodeNested(encoded, TypeValue.BytesValue);
            var hex     = Convert.ToHexString((actual.Value.ValueOf<BytesValue>()).Buffer);

            // Assert
            Assert.AreEqual(expected, hex);
        }
    }
}
