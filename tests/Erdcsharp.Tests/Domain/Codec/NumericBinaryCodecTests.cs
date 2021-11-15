using System;
using System.Numerics;
using Erdcsharp.Domain;
using Erdcsharp.Domain.Codec;
using Erdcsharp.Domain.Helper;
using Erdcsharp.Domain.Values;
using NUnit.Framework;

namespace Erdcsharp.UnitTests.Domain.Codec
{
    [TestFixture(Category = "UnitTests")]
    public class NumericBinaryCodecTests
    {
        private NumericBinaryCodec _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new NumericBinaryCodec();
        }

        private static void Check(string number, IBinaryType value, string actualHexEncoded, string expectedHexEncoded)
        {
            Assert.AreEqual(number, (value.ValueOf<NumericValue>()).Number.ToString());
            Assert.AreEqual(expectedHexEncoded, actualHexEncoded);
        }

        [TestCase(byte.MinValue, "")]
        [TestCase((byte) 1, "01")]
        [TestCase((byte) 42, "2A")]
        [TestCase(byte.MaxValue, "FF")]
        public void EncodeTopLevel_AndDecode_U8Value(byte number, string hexEncoded)
        {
            // Arrange
            var value = NumericValue.U8Value(number);
            // Act
            var encoded          = _sut.EncodeTopLevel(value);
            var actual           = _sut.DecodeTopLevel(encoded, value.Type);
            var actualHexEncoded = Converter.ToHexString(encoded);

            // Assert
            Check(number.ToString(), actual, actualHexEncoded, hexEncoded);
        }

        [TestCase(sbyte.MinValue, "80")]
        [TestCase((sbyte) 0, "")]
        [TestCase((sbyte) 1, "01")]
        [TestCase((sbyte) 42, "2A")]
        [TestCase(sbyte.MaxValue, "7F")]
        public void EncodeTopLevel_AndDecode_I8Value(sbyte number, string hexEncoded)
        {
            // Arrange
            var value = NumericValue.I8Value(number);
            // Act
            var encoded          = _sut.EncodeTopLevel(value);
            var actual           = _sut.DecodeTopLevel(encoded, value.Type);
            var actualHexEncoded = Convert.ToHexString(encoded);

            // Assert
            Check(number.ToString(), actual, actualHexEncoded, hexEncoded);
        }

        [TestCase((ushort) 0, "")]
        [TestCase((ushort) 1, "01")]
        [TestCase((ushort) 42, "2A")]
        [TestCase(ushort.MaxValue, "FFFF")]
        public void EncodeTopLevel_AndDecode_U16Value(ushort number, string hexEncoded)
        {
            // Arrange
            var value = NumericValue.U16Value(number);
            // Act
            var encoded          = _sut.EncodeTopLevel(value);
            var actual           = _sut.DecodeTopLevel(encoded, value.Type);
            var actualHexEncoded = Convert.ToHexString(encoded);

            // Assert
            Check(number.ToString(), actual, actualHexEncoded, hexEncoded);
        }

        [TestCase(short.MinValue, "8000")]
        [TestCase((short) 0, "")]
        [TestCase((short) 1, "01")]
        [TestCase((short) 42, "2A")]
        [TestCase(short.MaxValue, "7FFF")]
        public void EncodeTopLevel_AndDecode_I16Value(short number, string hexEncoded)
        {
            // Arrange
            var value = NumericValue.I16Value(number);
            // Act
            var encoded          = _sut.EncodeTopLevel(value);
            var actual           = _sut.DecodeTopLevel(encoded, value.Type);
            var actualHexEncoded = Convert.ToHexString(encoded);

            // Assert
            Check(number.ToString(), actual, actualHexEncoded, hexEncoded);
        }

        [TestCase(uint.MinValue, "")]
        [TestCase((uint) 1, "01")]
        [TestCase((uint) 42, "2A")]
        [TestCase(uint.MaxValue, "FFFFFFFF")]
        public void EncodeTopLevel_AndDecode_U32Value(uint number, string hexEncoded)
        {
            // Arrange
            var value = NumericValue.U32Value(number);
            // Act
            var encoded          = _sut.EncodeTopLevel(value);
            var actual           = _sut.DecodeTopLevel(encoded, value.Type);
            var actualHexEncoded = Convert.ToHexString(encoded);

            // Assert
            Check(number.ToString(), actual, actualHexEncoded, hexEncoded);
        }

        [TestCase(int.MinValue, "80000000")]
        [TestCase(0, "")]
        [TestCase(1, "01")]
        [TestCase(42, "2A")]
        [TestCase(int.MaxValue, "7FFFFFFF")]
        public void EncodeTopLevel_AndDecode_I32Value(int number, string hexEncoded)
        {
            // Arrange
            var value = NumericValue.I32Value(number);
            // Act
            var encoded          = _sut.EncodeTopLevel(value);
            var actual           = _sut.DecodeTopLevel(encoded, value.Type);
            var actualHexEncoded = Convert.ToHexString(encoded);

            // Assert
            Check(number.ToString(), actual, actualHexEncoded, hexEncoded);
        }

        [TestCase(ulong.MinValue, "")]
        [TestCase((ulong) 1, "01")]
        [TestCase((ulong) 42, "2A")]
        [TestCase(ulong.MaxValue, "FFFFFFFFFFFFFFFF")]
        public void EncodeTopLevel_AndDecode_U64Value(ulong number, string hexEncoded)
        {
            // Arrange
            var value = NumericValue.U64Value(number);
            // Act
            var encoded          = _sut.EncodeTopLevel(value);
            var actual           = _sut.DecodeTopLevel(encoded, value.Type);
            var actualHexEncoded = Convert.ToHexString(encoded);

            // Assert
            Check(number.ToString(), actual, actualHexEncoded, hexEncoded);
        }

        [TestCase(long.MinValue, "8000000000000000")]
        [TestCase((long) 0, "")]
        [TestCase((long) 1, "01")]
        [TestCase((long) 42, "2A")]
        [TestCase(long.MaxValue, "7FFFFFFFFFFFFFFF")]
        public void EncodeTopLevel_AndDecode_I64Value(long number, string hexEncoded)
        {
            // Arrange
            var value = NumericValue.I64Value(number);
            // Act
            var encoded          = _sut.EncodeTopLevel(value);
            var actual           = _sut.DecodeTopLevel(encoded, value.Type);
            var actualHexEncoded = Convert.ToHexString(encoded);

            // Assert
            Check(number.ToString(), actual, actualHexEncoded, hexEncoded);
        }

        [TestCase("0", "")]
        [TestCase("1", "01")]
        [TestCase("42", "2A")]
        [TestCase("1844674407370955161576567687", "05F5E0FFFFFFFFFFFE9A7387")]
        public void EncodeTopLevel_AndDecode_BigUintValue(string number, string hexEncoded)
        {
            // Arrange
            var value = NumericValue.BigUintValue(BigInteger.Parse(number));
            // Act
            var encoded          = _sut.EncodeTopLevel(value);
            var actual           = _sut.DecodeTopLevel(encoded, value.Type);
            var actualHexEncoded = Convert.ToHexString(encoded);

            // Assert
            Check(number.ToString(), actual, actualHexEncoded, hexEncoded);
        }

        [TestCase("-1844674407370955161576567687", "FA0A1F000000000001658C79")]
        [TestCase("0", "")]
        [TestCase("1", "01")]
        [TestCase("42", "2A")]
        [TestCase("1844674407370955161576567687", "05F5E0FFFFFFFFFFFE9A7387")]
        public void EncodeTopLevel_AndDecode_BigIntValue(string number, string hexEncoded)
        {
            // Arrange
            var value = NumericValue.BigIntValue(BigInteger.Parse(number));
            // Act
            var encoded          = _sut.EncodeTopLevel(value);
            var actual           = _sut.DecodeTopLevel(encoded, value.Type);
            var actualHexEncoded = Convert.ToHexString(encoded);

            // Assert
            Check(number.ToString(), actual, actualHexEncoded, hexEncoded);
        }
        // Nested

        [TestCase(byte.MinValue, "00")]
        [TestCase((byte) 1, "01")]
        [TestCase((byte) 42, "2A")]
        [TestCase(byte.MaxValue, "FF")]
        public void EncodeNested_AndDecode_U8Value(byte number, string hexEncoded)
        {
            // Arrange
            var value = NumericValue.U8Value(number);
            // Act
            var encoded          = _sut.EncodeNested(value);
            var actual           = _sut.DecodeNested(encoded, value.Type);
            var actualHexEncoded = Convert.ToHexString(encoded);

            // Assert
            Check(number.ToString(), actual.Value, actualHexEncoded, hexEncoded);
        }

        [TestCase(sbyte.MinValue, "80")]
        [TestCase((sbyte) 0, "00")]
        [TestCase((sbyte) 1, "01")]
        [TestCase((sbyte) 42, "2A")]
        [TestCase(sbyte.MaxValue, "7F")]
        public void EncodeNested_AndDecode_I8Value(sbyte number, string hexEncoded)
        {
            // Arrange
            var value = NumericValue.I8Value(number);
            // Act
            var encoded          = _sut.EncodeNested(value);
            var actual           = _sut.DecodeNested(encoded, value.Type);
            var actualHexEncoded = Convert.ToHexString(encoded);

            // Assert
            Check(number.ToString(), actual.Value, actualHexEncoded, hexEncoded);
        }

        [TestCase(ushort.MinValue, "0000")]
        [TestCase((ushort) 1, "0001")]
        [TestCase((ushort) 42, "002A")]
        [TestCase(ushort.MaxValue, "FFFF")]
        public void EncodeNested_AndDecode_U16Value(ushort number, string hexEncoded)
        {
            // Arrange
            var value = NumericValue.U16Value(number);
            // Act
            var encoded          = _sut.EncodeNested(value);
            var actual           = _sut.DecodeNested(encoded, value.Type);
            var actualHexEncoded = Convert.ToHexString(encoded);

            // Assert
            Check(number.ToString(), actual.Value, actualHexEncoded, hexEncoded);
        }

        [TestCase(short.MinValue, "8000")]
        [TestCase((short) 0, "0000")]
        [TestCase((short) 1, "0001")]
        [TestCase((short) 42, "002A")]
        [TestCase(short.MaxValue, "7FFF")]
        public void EncodeNested_AndDecode_I16Value(short number, string hexEncoded)
        {
            // Arrange
            var value = NumericValue.I16Value(number);
            // Act
            var encoded          = _sut.EncodeNested(value);
            var actual           = _sut.DecodeNested(encoded, value.Type);
            var actualHexEncoded = Convert.ToHexString(encoded);

            // Assert
            Check(number.ToString(), actual.Value, actualHexEncoded, hexEncoded);
        }

        [TestCase(uint.MinValue, "00000000")]
        [TestCase((uint) 1, "00000001")]
        [TestCase((uint) 42, "0000002A")]
        [TestCase(uint.MaxValue, "FFFFFFFF")]
        public void EncodeNested_AndDecode_U32Value(uint number, string hexEncoded)
        {
            // Arrange
            var value = NumericValue.U32Value(number);
            // Act
            var encoded          = _sut.EncodeNested(value);
            var actual           = _sut.DecodeNested(encoded, value.Type);
            var actualHexEncoded = Convert.ToHexString(encoded);

            // Assert
            Check(number.ToString(), actual.Value, actualHexEncoded, hexEncoded);
        }

        [TestCase(int.MinValue, "80000000")]
        [TestCase(0, "00000000")]
        [TestCase(1, "00000001")]
        [TestCase(42, "0000002A")]
        [TestCase(int.MaxValue, "7FFFFFFF")]
        public void EncodeNested_AndDecode_I32Value(int number, string hexEncoded)
        {
            // Arrange
            var value = NumericValue.I32Value(number);
            // Act
            var encoded          = _sut.EncodeNested(value);
            var actual           = _sut.DecodeNested(encoded, value.Type);
            var actualHexEncoded = Convert.ToHexString(encoded);

            // Assert
            Check(number.ToString(), actual.Value, actualHexEncoded, hexEncoded);
        }

        [TestCase(ulong.MinValue, "0000000000000000")]
        [TestCase((ulong) 1, "0000000000000001")]
        [TestCase((ulong) 42, "000000000000002A")]
        [TestCase(ulong.MaxValue, "FFFFFFFFFFFFFFFF")]
        public void EncodeNested_AndDecode_U64Value(ulong number, string hexEncoded)
        {
            // Arrange
            var value = NumericValue.U64Value(number);
            // Act
            var encoded          = _sut.EncodeNested(value);
            var actual           = _sut.DecodeNested(encoded, value.Type);
            var actualHexEncoded = Convert.ToHexString(encoded);

            // Assert
            Check(number.ToString(), actual.Value, actualHexEncoded, hexEncoded);
        }

        [TestCase(-922337203685477580, "F333333333333334")]
        [TestCase((long) 0, "0000000000000000")]
        [TestCase((long) 1, "0000000000000001")]
        [TestCase((long) 42, "000000000000002A")]
        [TestCase(922337203685477580, "0CCCCCCCCCCCCCCC")]
        public void EncodeNested_AndDecode_I64Value(long number, string hexEncoded)
        {
            // Arrange
            var value = NumericValue.I64Value(number);
            // Act
            var encoded          = _sut.EncodeNested(value);
            var actual           = _sut.DecodeNested(encoded, value.Type);
            var actualHexEncoded = Convert.ToHexString(encoded);

            // Assert
            Check(number.ToString(), actual.Value, actualHexEncoded, hexEncoded);
        }

        [TestCase("0", "00000000")]
        [TestCase("1", "0000000101")]
        [TestCase("42", "000000012A")]
        [TestCase("1844674407370955161576567687", "0000000C05F5E0FFFFFFFFFFFE9A7387")]
        public void EncodeNested_AndDecode_BigUintValue(string number, string hexEncoded)
        {
            // Arrange
            var value = NumericValue.BigUintValue(BigInteger.Parse(number));
            // Act
            var encoded          = _sut.EncodeNested(value);
            var actual           = _sut.DecodeNested(encoded, value.Type);
            var actualHexEncoded = Convert.ToHexString(encoded);

            // Assert
            Check(number, actual.Value, actualHexEncoded, hexEncoded);
        }

        [TestCase("-1844674407370955161576567687", "0000000CFA0A1F000000000001658C79")]
        [TestCase("0", "00000000")]
        [TestCase("1", "0000000101")]
        [TestCase("42", "000000012A")]
        [TestCase("1844674407370955161576567687", "0000000C05F5E0FFFFFFFFFFFE9A7387")]
        public void EncodeNested_AndDecode_BigIntValue(string number, string hexEncoded)
        {
            // Arrange
            var value = NumericValue.BigIntValue(BigInteger.Parse(number));
            // Act
            var encoded          = _sut.EncodeNested(value);
            var actual           = _sut.DecodeNested(encoded, value.Type);
            var actualHexEncoded = Convert.ToHexString(encoded);

            // Assert
            Check(number, actual.Value, actualHexEncoded, hexEncoded);
        }

        [Test]
        public void EncodeNested_DecodeNested_BalanceValue()
        {
            // Arrange
            var value = NumericValue.TokenAmount(TokenAmount.EGLD("12.876564"));

            // Act
            var encoded = _sut.EncodeNested(value);
            var actual  = _sut.DecodeNested(encoded, value.Type);

            // Assert
            var balance = actual.Value.ValueOf<NumericValue>();
            Assert.That(balance.Number, Is.EqualTo(value.Number));
        }
    }
}
