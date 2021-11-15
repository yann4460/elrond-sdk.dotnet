using System;
using System.Collections.Generic;
using Erdcsharp.Domain;
using Erdcsharp.Domain.Codec;
using Erdcsharp.Domain.Values;
using NUnit.Framework;

namespace Erdcsharp.UnitTests.Domain.Codec
{
    [TestFixture(Category = "UnitTests")]
    public class MultiBinaryCodecTests
    {
        private MultiBinaryCodec _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new MultiBinaryCodec(new BinaryCodec());
        }

        [Test]
        public void EncodeTopLevel()
        {
            // Arrange
            var value = MultiValue.From(
                                        NumericValue.BigUintValue(TokenAmount.From("100000000000000000").Value),
                                        NumericValue.BigUintValue(TokenAmount.From("10000000000000000000").Value)
                                       );

            // Act
            var actual = _sut.EncodeTopLevel(value);
            var hex    = Convert.ToHexString(actual);

            // Assert
            Assert.That(hex, Is.EqualTo("00000008016345785D8A0000000000088AC7230489E80000"));
        }

        [Test]
        public void EncodeNested()
        {
            // Arrange
            var value = MultiValue.From(
                                        NumericValue.BigUintValue(TokenAmount.From("100000000000000000").Value),
                                        NumericValue.BigUintValue(TokenAmount.From("10000000000000000000").Value)
                                       );

            // Act
            var actual = _sut.EncodeNested(value);
            var hex    = Convert.ToHexString(actual);

            // Assert
            Assert.That(hex, Is.EqualTo("00000008016345785D8A0000000000088AC7230489E80000"));
        }

        [Test]
        public void DecodeTopLevel()
        {
            // Arrange
            var a     = "00000008016345785D8A0000";
            var b     = "000000088AC7230489E80000";
            var bytes = new List<byte>();
            bytes.AddRange(Convert.FromHexString(a));
            bytes.AddRange(Convert.FromHexString(b));

            //// Act
            var actual = _sut.DecodeTopLevel(bytes.ToArray(),
                                             TypeValue.MultiValue(new[] {TypeValue.BigUintTypeValue, TypeValue.BigUintTypeValue}));

            var values = actual.ValueOf<MultiValue>().Values;

            //// Assert
            //Assert.That(hex, Is.EqualTo("00"));
        }
    }
}
