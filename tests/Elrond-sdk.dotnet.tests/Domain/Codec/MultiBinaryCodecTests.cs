using System;
using System.Collections.Generic;
using System.Numerics;
using Elrond.Dotnet.Sdk.Domain.Codec;
using Elrond.Dotnet.Sdk.Domain.Values;
using NUnit.Framework;

namespace Elrond_sdk.dotnet.tests.Domain.Codec
{
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
                NumericValue.BigUintValue(Balance.From("100000000000000000").Number),
                NumericValue.BigUintValue(Balance.From("10000000000000000000").Number)
            );

            // Act
            var actual = _sut.EncodeTopLevel(value);
            var hex = Convert.ToHexString(actual);

            // Assert
            Assert.That(hex, Is.EqualTo("00000008016345785D8A0000000000088AC7230489E80000"));
        }

        [Test]
        public void EncodeNested()
        {
            // Arrange
            var value = MultiValue.From(
                NumericValue.BigUintValue(Balance.From("100000000000000000").Number),
                NumericValue.BigUintValue(Balance.From("10000000000000000000").Number)
            );

            // Act
            var actual = _sut.EncodeNested(value);
            var hex = Convert.ToHexString(actual);

            // Assert
            Assert.That(hex, Is.EqualTo("00000008016345785D8A0000000000088AC7230489E80000"));
        }

        [Test]
        public void DecodeTopLevel()
        {
            // Arrange
            var a = "00000008016345785D8A0000";
            var b = "000000088AC7230489E80000";
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