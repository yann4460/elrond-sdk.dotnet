using System;
using System.Numerics;
using Elrond.Dotnet.Sdk.Domain.Codec;
using Erdcsharp.Domain;
using Erdcsharp.Domain.Codec;
using Erdcsharp.Domain.Values;
using NUnit.Framework;

namespace Erdcsharp.UnitTests.Domain.Codec
{
    [TestFixture(Category = "UnitTests")]
    public class EnumBinaryCodecTests
    {
        private EnumBinaryCodec _sut;
        private TypeValue _enumValue;

        [SetUp]
        public void Setup()
        {
            _sut = new EnumBinaryCodec(new BinaryCodec());
            var monday    = new EnumVariantDefinition("Monday", 0);
            var tuesday   = new EnumVariantDefinition("Tuesday", 1);
            var wednesday = new EnumVariantDefinition("Wednesday", 1);
            var thursday  = new EnumVariantDefinition("Thursday", 3);
            _enumValue = TypeValue.EnumValue("enum", new[] { monday, tuesday, wednesday, thursday });
        }

        [Test]
        public void EncodeTopLevel_First_Discriminant()
        {
            // Arrange
            var monday = _enumValue.GetVariantByName("Monday");

            // Act
            var actual    = _sut.EncodeTopLevel(new EnumValue(_enumValue, monday));
            var hexEncode = Convert.ToHexString(actual);

            // Assert
            Assert.That(hexEncode, Is.EqualTo(""));
        }

        [Test]
        public void EncodeNested_First_Discriminant()
        {
            // Arrange
            var monday = _enumValue.GetVariantByName("Monday");

            // Act
            var actual    = _sut.EncodeNested(new EnumValue(_enumValue, monday));
            var hexEncode = Convert.ToHexString(actual);

            // Assert
            Assert.That(hexEncode, Is.EqualTo("00"));
        }

        [Test]
        public void EncodeTopLevel_Second_Discriminant()
        {
            // Arrange
            var monday = _enumValue.GetVariantByName("Tuesday");

            // Act
            var actual    = _sut.EncodeTopLevel(new EnumValue(_enumValue, monday));
            var hexEncode = Convert.ToHexString(actual);

            // Assert
            Assert.That(hexEncode, Is.EqualTo("01"));
        }

        [Test]
        public void EncodeNested_Second_Discriminant()
        {
            // Arrange
            var monday = _enumValue.GetVariantByName("Tuesday");

            // Act
            var actual    = _sut.EncodeNested(new EnumValue(_enumValue, monday));
            var hexEncode = Convert.ToHexString(actual);

            // Assert
            Assert.That(hexEncode, Is.EqualTo("01"));
        }
    }
}
