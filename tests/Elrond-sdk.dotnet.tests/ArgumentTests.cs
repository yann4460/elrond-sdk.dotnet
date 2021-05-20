using Elrond.Dotnet.Sdk.Domain;
using Elrond.Dotnet.Sdk.Domain.Values;
using NUnit.Framework;

namespace Elrond_sdk.dotnet.tests
{
    public class ArgumentTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CreateArgumentFromAddress()
        {
            // Arrange
            var address = AddressValue.FromBech32("erd1lkeja8knfjhkqzvrf3d9hmefxzt75wtf3vlg9m7ccugc8jmnrdpqy7yjeq");

            // Act
            var actual = Argument.CreateArgumentFromAddress(address);

            // Assert
            Assert.AreEqual("fdb32e9ed34caf6009834c5a5bef293097ea39698b3e82efd8c71183cb731b42", actual.Value);
        }


        [Test]
        public void CreateArgumentFromHex()
        {
            // Arrange
            var hexString = "436563692065737420756e2074657374"; //Hex string

            // Act
            var actual = Argument.CreateArgumentFromHex(hexString);

            // Assert
            Assert.AreEqual(hexString, actual.Value);
        }

        [Test]
        public void CreateArgumentFromBoolean_True()
        {
            // Act
            var actual = Argument.CreateArgumentFromBoolean(true);

            // Assert
            Assert.AreEqual("01", actual.Value);
        }

        [Test]
        public void CreateArgumentFromBoolean_False()
        {
            // Act
            var actual = Argument.CreateArgumentFromBoolean(false);

            // Assert
            Assert.AreEqual("00", actual.Value);
        }

        [Test]
        public void CreateArgumentFromBalance()
        {
            // Arrange
            var low = Balance.EGLD("0.123456789123415267");
            var zero = Balance.Zero();
            var high = Balance.EGLD("31000000.123456789123415267");

            // Act
            var actualLow = Argument.CreateArgumentFromBalance(low);
            var actualZero = Argument.CreateArgumentFromBalance(zero);
            var actualHigh = Argument.CreateArgumentFromBalance(high);

            // Assert
            Assert.AreEqual("01b69b4baccfbce3", actualLow.Value);
            Assert.AreEqual("", actualZero.Value);
            Assert.AreEqual("19a4815fc16c122bcfbce3", actualHigh.Value);
        }

        [Test]
        public void CreateArgumentFromUtf8String()
        {
            // Arrange
            var stringValue = "TSTKR-ce4c6d";

            // Act
            var actual = Argument.CreateArgumentFromUtf8String(stringValue);

            // Assert
            Assert.AreEqual("5453544b522d636534633664", actual.Value);
        }

        [Test]
        public void CreateArgumentFromBalance_LongDecimalValue()
        {
            // Arrange
            var balance = Balance.EGLD("0.123456789112345");

            // Act
            var actual = Argument.CreateArgumentFromBalance(balance);

            // Assert
            Assert.AreEqual("01b69b4bac26d1a8", actual.Value);
        }

        [Test]
        public void CreateArgumentFromBalance_LongDecimalValue2()
        {
            // Arrange
            var balance = Balance.EGLD("100.87867564345465787");

            // Act
            var actual = Argument.CreateArgumentFromBalance(balance);

            // Assert
            Assert.AreEqual("0577f90d0503f7154e", actual.Value);
        }

        [Test]
        public void CreateArgumentFromInt16()
        {
            // Act
            var minValue = Argument.CreateArgumentFromInt16(short.MinValue);
            var zero = Argument.CreateArgumentFromInt16(0);
            var maxValue = Argument.CreateArgumentFromInt16(short.MaxValue);

            var opt_minValue = Argument.CreateArgumentFromInt16(short.MinValue, true);
            var opt_zero = Argument.CreateArgumentFromInt16(0, true);
            var opt_one= Argument.CreateArgumentFromInt16(1, true);
            var opt_maxValue = Argument.CreateArgumentFromInt16(short.MaxValue, true);

            // Assert
            Assert.AreEqual("8000", minValue.Value);
            Assert.AreEqual("", zero.Value);
            Assert.AreEqual("7fff", maxValue.Value);

            Assert.AreEqual("018000", opt_minValue.Value);
            Assert.AreEqual("010000", opt_zero.Value);
            Assert.AreEqual("010001", opt_one.Value);
            Assert.AreEqual("017fff", opt_maxValue.Value);
        }

        [Test]
        public void CreateArgumentFromUInt16()
        {
            // Act
            var minValue = Argument.CreateArgumentFromUInt16(ushort.MinValue);
            var randValue = Argument.CreateArgumentFromUInt16(42);
            var maxValue = Argument.CreateArgumentFromUInt16(ushort.MaxValue);

            var opt_minValue = Argument.CreateArgumentFromUInt16(ushort.MinValue, true);
            var opt_one = Argument.CreateArgumentFromUInt16(1, true);
            var opt_maxValue = Argument.CreateArgumentFromUInt16(ushort.MaxValue, true);

            // Assert
            Assert.AreEqual("", minValue.Value);
            Assert.AreEqual("2a", randValue.Value);
            Assert.AreEqual("ffff", maxValue.Value);

            Assert.AreEqual("010000", opt_minValue.Value);
            Assert.AreEqual("010001", opt_one.Value);
            Assert.AreEqual("01ffff", opt_maxValue.Value);
        }
        
        [Test]
        public void CreateArgumentFromInt32()
        {
            // Act
            var minValue = Argument.CreateArgumentFromInt32(int.MinValue);
            var zero = Argument.CreateArgumentFromInt32(0);
            var maxValue = Argument.CreateArgumentFromInt32(int.MaxValue);

            var opt_minValue = Argument.CreateArgumentFromInt32(int.MinValue, true);
            var opt_zero = Argument.CreateArgumentFromInt32(0, true);
            var opt_one = Argument.CreateArgumentFromInt32(1, true);
            var opt_maxValue = Argument.CreateArgumentFromInt32(int.MaxValue, true);

            // Assert
            Assert.AreEqual("80000000", minValue.Value);
            Assert.AreEqual("", zero.Value);
            Assert.AreEqual("7fffffff", maxValue.Value);

            Assert.AreEqual("0180000000", opt_minValue.Value);
            Assert.AreEqual("0100000000", opt_zero.Value);
            Assert.AreEqual("0100000001", opt_one.Value);
            Assert.AreEqual("017fffffff", opt_maxValue.Value);
        }

        [Test]
        public void CreateArgumentFromUInt32()
        {
            // Act
            var minValue = Argument.CreateArgumentFromUInt32(uint.MinValue);
            var randValue = Argument.CreateArgumentFromUInt16(42);
            var maxValue = Argument.CreateArgumentFromUInt32(uint.MaxValue);

            var opt_minValue = Argument.CreateArgumentFromUInt32(uint.MinValue, true);
            var opt_one = Argument.CreateArgumentFromUInt32(1, true);
            var opt_maxValue = Argument.CreateArgumentFromUInt32(uint.MaxValue, true);

            // Assert
            Assert.AreEqual("", minValue.Value);
            Assert.AreEqual("2a", randValue.Value);
            Assert.AreEqual("ffffffff", maxValue.Value);

            Assert.AreEqual("0100000000", opt_minValue.Value);
            Assert.AreEqual("0100000001", opt_one.Value);
            Assert.AreEqual("01ffffffff", opt_maxValue.Value);
        }

        //@@01@ffffffffffffffff@010000000000000000@010000000000000001@01ffffffffffffffff
        [Test]
        public void CreateArgumentFromInt64()
        {
            // Act
            var minValue = Argument.CreateArgumentFromInt64(-9223372036854775808);
            var zero = Argument.CreateArgumentFromInt64(0);
            var one = Argument.CreateArgumentFromInt64(1);
            var maxValue = Argument.CreateArgumentFromInt64(long.MaxValue);

            var opt_minValue = Argument.CreateArgumentFromInt64(long.MinValue, true);
            var opt_zero = Argument.CreateArgumentFromInt64(0, true);
            var opt_one = Argument.CreateArgumentFromInt64(1, true);
            var opt_maxValue = Argument.CreateArgumentFromInt64(long.MaxValue, true);

            // Assert
            Assert.AreEqual("8000000000000000", minValue.Value);
            Assert.AreEqual("", zero.Value);
            Assert.AreEqual("01", one.Value);
            Assert.AreEqual("7fffffffffffffff", maxValue.Value);

            Assert.AreEqual("018000000000000000", opt_minValue.Value);
            Assert.AreEqual("010000000000000000", opt_zero.Value);
            Assert.AreEqual("010000000000000001", opt_one.Value);
            Assert.AreEqual("017fffffffffffffff", opt_maxValue.Value);
        }
        //@@01@ffffffffffffffff@010000000000000000@010000000000000001@01ffffffffffffffff
        [Test]
        public void CreateArgumentFromUInt64()
        {
            // Act
            var minValue = Argument.CreateArgumentFromUInt64(ulong.MinValue);
            var oneValue = Argument.CreateArgumentFromUInt64(1);
            var maxValue = Argument.CreateArgumentFromUInt64(ulong.MaxValue);

            var opt_minValue = Argument.CreateArgumentFromUInt64(ulong.MinValue, true);
            var opt_one = Argument.CreateArgumentFromUInt64(1, true);
            var opt_maxValue = Argument.CreateArgumentFromUInt64(18446744073709551615, true);

            // Assert
            Assert.AreEqual("", minValue.Value);
            Assert.AreEqual("01", oneValue.Value);
            Assert.AreEqual("ffffffffffffffff", maxValue.Value);

            Assert.AreEqual("010000000000000000", opt_minValue.Value);
            Assert.AreEqual("010000000000000001", opt_one.Value);
            Assert.AreEqual("01ffffffffffffffff", opt_maxValue.Value);
        }
    }
}