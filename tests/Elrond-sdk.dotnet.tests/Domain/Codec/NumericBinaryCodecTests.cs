using System;
using Elrond.Dotnet.Sdk.Domain.Codec;
using NUnit.Framework;

namespace Elrond_sdk.dotnet.tests.Domain.Codec
{
    public class NumericBinaryCodecTests
    {
        private NumericBinaryCodec _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new NumericBinaryCodec();
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
            var encoded = _sut.EncodeTopLevel(value);
            var actual = _sut.DecodeTopLevel(encoded, value.Type);
            var actualHexEncoded = Convert.ToHexString(encoded);
            // Assert
            Assert.AreEqual(number.ToString(), actual.ValueOf().ToString());
            Assert.AreEqual(hexEncoded, actualHexEncoded);
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
            var encoded = _sut.EncodeTopLevel(value);
            var actual = _sut.DecodeTopLevel(encoded, value.Type);
            var actualHexEncoded = Convert.ToHexString(encoded);

            // Assert
            Assert.AreEqual(number.ToString(), actual.ValueOf().ToString());
            Assert.AreEqual(hexEncoded, actualHexEncoded);
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
            var encoded = _sut.EncodeTopLevel(value);
            var actual = _sut.DecodeTopLevel(encoded, value.Type);
            var actualHexEncoded = Convert.ToHexString(encoded);

            // Assert
            Assert.AreEqual(number.ToString(), actual.ValueOf().ToString());
            Assert.AreEqual(hexEncoded, actualHexEncoded);
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
            var encoded = _sut.EncodeTopLevel(value);
            var actual = _sut.DecodeTopLevel(encoded, value.Type);
            var actualHexEncoded = Convert.ToHexString(encoded);

            // Assert
            Assert.AreEqual(number.ToString(), actual.ValueOf().ToString());
            Assert.AreEqual(hexEncoded, actualHexEncoded);
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
            var encoded = _sut.EncodeTopLevel(value);
            var actual = _sut.DecodeTopLevel(encoded, value.Type);
            var actualHexEncoded = Convert.ToHexString(encoded);

            // Assert
            Assert.AreEqual(number.ToString(), actual.ValueOf().ToString());
            Assert.AreEqual(hexEncoded, actualHexEncoded);
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
            var encoded = _sut.EncodeTopLevel(value);
            var actual = _sut.DecodeTopLevel(encoded, value.Type);
            var actualHexEncoded = Convert.ToHexString(encoded);

            // Assert
            Assert.AreEqual(number.ToString(), actual.ValueOf().ToString());
            Assert.AreEqual(hexEncoded, actualHexEncoded);
        }

        [TestCase(ulong.MinValue)]
        [TestCase((ulong) 1)]
        [TestCase((ulong) 42)]
        [TestCase(ulong.MaxValue)]
        public void EncodeTopLevel_AndDecode_U64Value(ulong number)
        {
            // Arrange
            var value = NumericValue.U64Value(number);
            // Act
            var encoded = _sut.EncodeTopLevel(value);
            var actual = _sut.DecodeTopLevel(encoded, value.Type);

            // Assert
            Assert.AreEqual(number.ToString(), actual.ValueOf().ToString());
        }

        [TestCase(long.MinValue)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(42)]
        [TestCase(long.MaxValue)]
        public void EncodeTopLevel_AndDecode_I64Value(long number)
        {
            // Arrange
            var value = NumericValue.I64Value(number);
            // Act
            var encoded = _sut.EncodeTopLevel(value);
            var actual = _sut.DecodeTopLevel(encoded, value.Type);

            // Assert
            Assert.AreEqual(number.ToString(), actual.ValueOf().ToString());
        }


        // Nested

        [TestCase(byte.MinValue, "00")]
        [TestCase((byte)1, "01")]
        [TestCase((byte)42, "2A")]
        [TestCase(byte.MaxValue, "FF")]
        public void EncodeNested_AndDecode_U8Value(byte number, string hexEncoded)
        {
            // Arrange
            var value = NumericValue.U8Value(number);
            // Act
            var encoded = _sut.EncodeNested(value);
            var actual = _sut.DecodeNested(encoded, value.Type);
            var actualHexEncoded = Convert.ToHexString(encoded);

            // Assert
            Assert.AreEqual(number.ToString(), actual.Value.ValueOf().ToString());
            Assert.AreEqual(hexEncoded, actualHexEncoded);
        }

        [TestCase(sbyte.MinValue, "80")]
        [TestCase((sbyte)0, "00")]
        [TestCase((sbyte)1, "01")]
        [TestCase((sbyte)42, "2A")]
        [TestCase(sbyte.MaxValue, "7F")]
        public void EncodeNested_AndDecode_I8Value(sbyte number, string hexEncoded)
        {
            // Arrange
            var value = NumericValue.I8Value(number);
            // Act
            var encoded = _sut.EncodeNested(value);
            var actual = _sut.DecodeNested(encoded, value.Type);
            var actualHexEncoded = Convert.ToHexString(encoded);

            // Assert
            Assert.AreEqual(number.ToString(), actual.Value.ValueOf().ToString());
            Assert.AreEqual(hexEncoded, actualHexEncoded);
        }

        [TestCase(ushort.MinValue, "0000")]
        [TestCase((ushort)1, "0001")]
        [TestCase((ushort)42, "002A")]
        [TestCase(ushort.MaxValue, "FFFF")]
        public void EncodeNested_AndDecode_U16Value(ushort number, string hexEncoded)
        {
            // Arrange
            var value = NumericValue.U16Value(number);
            // Act
            var encoded = _sut.EncodeNested(value);
            var actual = _sut.DecodeNested(encoded, value.Type);
            var actualHexEncoded = Convert.ToHexString(encoded);

            // Assert
            Assert.AreEqual(number.ToString(), actual.Value.ValueOf().ToString());
            Assert.AreEqual(hexEncoded, actualHexEncoded);
        }

        [TestCase(short.MinValue, "8000")]
        [TestCase((short)0, "0000")]
        [TestCase((short)1, "0001")]
        [TestCase((short)42, "002A")]
        [TestCase(short.MaxValue, "7FFF")]
        public void EncodeNested_AndDecode_I16Value(short number, string hexEncoded)
        {
            // Arrange
            var value = NumericValue.I16Value(number);
            // Act
            var encoded = _sut.EncodeNested(value);
            var actual = _sut.DecodeNested(encoded, value.Type);
            var actualHexEncoded = Convert.ToHexString(encoded);

            // Assert
            Assert.AreEqual(number.ToString(), actual.Value.ValueOf().ToString());
            Assert.AreEqual(hexEncoded, actualHexEncoded);
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
            var encoded = _sut.EncodeNested(value);
            var actual = _sut.DecodeNested(encoded, value.Type);
            var actualHexEncoded = Convert.ToHexString(encoded);

            // Assert
            Assert.AreEqual(number.ToString(), actual.Value.ValueOf().ToString());
            Assert.AreEqual(hexEncoded, actualHexEncoded);
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
            var encoded = _sut.EncodeNested(value);
            var actual = _sut.DecodeNested(encoded, value.Type);
            var actualHexEncoded = Convert.ToHexString(encoded);

            // Assert
            Assert.AreEqual(number.ToString(), actual.Value.ValueOf().ToString());
            Assert.AreEqual(hexEncoded, actualHexEncoded);
        }

        [TestCase(ulong.MinValue)]
        [TestCase((ulong) 1)]
        [TestCase((ulong) 42)]
        [TestCase(ulong.MaxValue)]
        public void EncodeNested_AndDecode_U64Value(ulong number)
        {
            // Arrange
            var value = NumericValue.U64Value(number);
            // Act
            var encoded = _sut.EncodeNested(value);
            var actual = _sut.DecodeNested(encoded, value.Type);

            // Assert
            Assert.AreEqual(number.ToString(), actual.Value.ValueOf().ToString());
        }

        [TestCase(long.MinValue)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(42)]
        [TestCase(long.MaxValue)]
        public void EncodeNested_AndDecode_I64Value(long number)
        {
            // Arrange
            var value = NumericValue.I64Value(number);
            // Act
            var encoded = _sut.EncodeNested(value);
            var actual = _sut.DecodeNested(encoded, value.Type);

            // Assert
            Assert.AreEqual(number.ToString(), actual.Value.ValueOf().ToString());
        }
    }
}