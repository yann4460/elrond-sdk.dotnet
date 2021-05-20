using System;
using System.Numerics;
using Elrond.Dotnet.Sdk.Domain;
using Elrond.Dotnet.Sdk.Domain.Codec;
using Elrond.Dotnet.Sdk.Domain.Values;
using NUnit.Framework;

namespace Elrond_sdk.dotnet.tests.Domain.Codec
{
    public class StructBinaryCodecTests
    {
        private StructBinaryCodec _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new StructBinaryCodec();
        }

        [Test]
        public void Encode_Simple_Value()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition("azeazeaze", "", TypeValue.RustTypes.u16);
            var structField = new StructField(NumericValue.U16Value(12), "azeazeaze");
            var type = TypeValue.StructValue("azeae", new[] {fieldDefinition});

            // Act
            var actual = _sut.EncodeNested(new StructValue(type, new[] {structField}));
            var hexEncode = Convert.ToHexString(actual);

            // Assert
            Assert.That(hexEncode, Is.EqualTo("000C"));
        }

        [Test]
        public void Encode_Complex_Value()
        {
            // Arrange
            var type = TypeValue.StructValue("Foo", new[]
            {
                new FieldDefinition("ticket_price", "", TypeValue.RustTypes.BigUint),
                new FieldDefinition("tickets_left", "", TypeValue.RustTypes.u32),
                new FieldDefinition("deadline", "", TypeValue.RustTypes.u64),
                new FieldDefinition("max_entries_per_user", "", TypeValue.RustTypes.u32),
                new FieldDefinition("prize_distribution", "", TypeValue.RustTypes.Bytes),
                new FieldDefinition("current_ticket_number", "", TypeValue.RustTypes.u32),
                new FieldDefinition("prize_pool", "", TypeValue.RustTypes.BigUint)
            });
            var structValue = new StructValue(type, new[]
            {
                new StructField(NumericValue.BigUintValue(Balance.EGLD("10").Value), "ticket_price"),
                new StructField(NumericValue.U32Value(0), "tickets_left"),
                new StructField(NumericValue.U64Value(0x000000005fc2b9db), "deadline"),
                new StructField(NumericValue.U32Value(0xffffffff), "max_entries_per_user"),
                new StructField(new BytesValue(new byte[] {0x64},TypeValue.BytesValue), "prize_distribution"),
                new StructField(NumericValue.U32Value(9472), "current_ticket_number"),
                new StructField(NumericValue.BigUintValue(BigInteger.Parse("94720000000000000000000")), "prize_pool"),
            });

            // Act
            var actual = _sut.EncodeNested(structValue);
            var hexEncode = Convert.ToHexString(actual);

            var decode = _sut.DecodeTopLevel(actual, type);

            // Assert
            Assert.That(hexEncode, Is.EqualTo("000000088AC7230489E8000000000000000000005FC2B9DBFFFFFFFF0000000164000025000000000A140EC80FA7EE88000000"));
        }

        [Test]
        public void Encode_Complex_Value2()
        {
            var hex =
                "0000000445474c44000000000000000000000008016345785d8a0000000000088ac7230489e800000000000060a44594fdb32e9ed34caf6009834c5a5bef293097ea39698b3e82efd8c71183cb731b420000000000000000000000000000000000000000000000000000000000000000000000000000000201f400000000";
            var data = Convert.FromHexString(hex);

            // Arrange
            var type = TypeValue.StructValue("Auction", new[]
            {
                new FieldDefinition("token_type", "", TypeValue.RustTypes.TokenIdentifier),
                new FieldDefinition("nonce", "", TypeValue.RustTypes.u64),
                new FieldDefinition("min_bid", "", TypeValue.RustTypes.BigUint),
                new FieldDefinition("max_bid", "", TypeValue.RustTypes.BigUint),
                new FieldDefinition("deadline", "", TypeValue.RustTypes.u64),
                new FieldDefinition("original_owner", "", TypeValue.RustTypes.Address),
                new FieldDefinition("current_bid", "", TypeValue.RustTypes.BigUint),
                new FieldDefinition("current_winner", "", TypeValue.RustTypes.Address),
                new FieldDefinition("marketplace_cut_percentage", "", TypeValue.RustTypes.BigUint),
                new FieldDefinition("creator_royalties_percentage", "", TypeValue.RustTypes.BigUint),
            });

            // Act

            var decode = _sut.DecodeTopLevel(data, type);
        }
    }
}