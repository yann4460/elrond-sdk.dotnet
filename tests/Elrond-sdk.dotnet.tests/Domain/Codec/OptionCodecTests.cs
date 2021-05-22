using System;
using Elrond.Dotnet.Sdk.Domain.Codec;
using Elrond.Dotnet.Sdk.Domain.Values;
using NUnit.Framework;

namespace Elrond_sdk.dotnet.tests.Domain.Codec
{
    public class OptionBinaryCodecTests
    {
        private OptionBinaryCodec _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new OptionBinaryCodec(new BinaryCodec());
        }

        [Test]
        public void EncodeTopLevel_NewMissing()
        {
            // Arrange
            var optionValue = OptionValue.NewMissing();

            // Act
            var actual = _sut.EncodeTopLevel(optionValue);
            var hex = Convert.ToHexString(actual);

            // Assert
            Assert.That(hex, Is.EqualTo(""));
        }

        [Test]
        public void EncodeNested_NewMissing()
        {
            // Arrange
            var optionValue = OptionValue.NewMissing();

            // Act
            var actual = _sut.EncodeNested(optionValue);
            var hex = Convert.ToHexString(actual);

            // Assert
            Assert.That(hex, Is.EqualTo("00"));
        }

        [Test]
        public void EncodeTopLevel_NewProvided()
        {
            // Arrange
            var optionValue = OptionValue.NewProvided(NumericValue.I16Value(12));

            // Act
            var actual = _sut.EncodeTopLevel(optionValue);
            var hex = Convert.ToHexString(actual);

            // Assert
            Assert.That(hex, Is.EqualTo("01000C"));
        }

        [Test]
        public void EncodeNested_NewProvided()
        {
            // Arrange
            var optionValue = OptionValue.NewProvided(NumericValue.I16Value(12));

            // Act
            var actual = _sut.EncodeNested(optionValue);
            var hex = Convert.ToHexString(actual);

            // Assert
            Assert.That(hex, Is.EqualTo("01000C"));
        }

    }
}