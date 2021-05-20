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
            _sut = new StructBinaryCodec(new BinaryCodec());
        }

        [Test]
        public void Encode_Simple_Value()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition("azeazeaze", "", TypeValue.U16TypeValue);
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
                new FieldDefinition("ticket_price", "", TypeValue.BigUintTypeValue),
                new FieldDefinition("tickets_left", "", TypeValue.U32TypeValue),
                new FieldDefinition("deadline", "", TypeValue.U64TypeValue),
                new FieldDefinition("max_entries_per_user", "", TypeValue.U32TypeValue),
                new FieldDefinition("prize_distribution", "", TypeValue.BigUintTypeValue),
                new FieldDefinition("current_ticket_number", "", TypeValue.U32TypeValue),
                new FieldDefinition("prize_pool", "", TypeValue.BigUintTypeValue)
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
            var base64= "AAAABEVHTEQAAAAAAAAAAAAAAAgBY0V4XYoAAAAAAAiKxyMEiegAAAAAAABgpEWU/bMuntNMr2AJg0xaW+8pMJfqOWmLPoLv2McRg8tzG0IAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACAfQAAAAA";
            var data = Convert.FromBase64String(base64);

            // Arrange
            var type = TypeValue.StructValue("Auction", new[]
            {
                new FieldDefinition("EsdtToken", "", TypeValue.StructValue("EsdtToken", new []
                {
                    new FieldDefinition("token_type","",TypeValue.TokenIdentifierValue), 
                    new FieldDefinition("nonce","",TypeValue.U64TypeValue),
                })),
                new FieldDefinition("min_bid", "", TypeValue.BigUintTypeValue),
                new FieldDefinition("max_bid", "", TypeValue.BigUintTypeValue),
                new FieldDefinition("deadline", "", TypeValue.U64TypeValue),
                new FieldDefinition("original_owner", "", TypeValue.AddressValue),
                new FieldDefinition("current_bid", "", TypeValue.BigUintTypeValue),
                new FieldDefinition("current_winner", "", TypeValue.AddressValue),
                new FieldDefinition("marketplace_cut_percentage", "", TypeValue.BigUintTypeValue),
                new FieldDefinition("creator_royalties_percentage", "", TypeValue.BigUintTypeValue),
            });

            // Act

            var decode = _sut.DecodeTopLevel(data, type);
        }
    }
}