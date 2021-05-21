﻿using System;
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
                new FieldDefinition("prize_distribution", "", TypeValue.BytesValue),
                new FieldDefinition("current_ticket_number", "", TypeValue.U32TypeValue),
                new FieldDefinition("prize_pool", "", TypeValue.BigUintTypeValue)
            });
            var structValue = new StructValue(type, new[]
            {
                new StructField(NumericValue.BigUintValue(BalanceValue.EGLD("10").Number), "ticket_price"),
                new StructField(NumericValue.U32Value(0), "tickets_left"),
                new StructField(NumericValue.U64Value(0x000000005fc2b9db), "deadline"),
                new StructField(NumericValue.U32Value(0xffffffff), "max_entries_per_user"),
                new StructField(BytesValue.FromBuffer(new byte[] {0x64}), "prize_distribution"),
                new StructField(NumericValue.U32Value(9472), "current_ticket_number"),
                new StructField(NumericValue.BigUintValue(BigInteger.Parse("94720000000000000000000")), "prize_pool"),
            });

            // Act
            var actual = _sut.EncodeNested(structValue);
            var hexEncode = Convert.ToHexString(actual);

            // Assert
            Assert.That(hexEncode,
                Is.EqualTo(
                    "000000088AC7230489E8000000000000000000005FC2B9DBFFFFFFFF0000000164000025000000000A140EC80FA7EE88000000"));
        }

        [Test]
        public void Encode_Complex_Value2()
        {
            var base64 =
                "AAAABEVHTEQAAAAAAAAAAAAAAAgBY0V4XYoAAAAAAAiKxyMEiegAAAAAAABgpEWU/bMuntNMr2AJg0xaW+8pMJfqOWmLPoLv2McRg8tzG0IAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACAfQAAAAA";
            var data = Convert.FromBase64String(base64);

            // Arrange
            var esdtToken = TypeValue.StructValue("EsdtToken", new[]
            {
                new FieldDefinition("token_type", "", TypeValue.TokenIdentifierValue),
                new FieldDefinition("nonce", "", TypeValue.U64TypeValue)
            });

            var auction = TypeValue.StructValue("Auction", new[]
            {
                new FieldDefinition("payment_token", "", esdtToken),
                new FieldDefinition("min_bid", "", TypeValue.BigUintTypeValue),
                new FieldDefinition("max_bid", "", TypeValue.BigUintTypeValue),
                new FieldDefinition("deadline", "", TypeValue.U64TypeValue),
                new FieldDefinition("original_owner", "", TypeValue.AddressValue),
                new FieldDefinition("current_bid", "", TypeValue.BigUintTypeValue),
                new FieldDefinition("current_winner", "", TypeValue.AddressValue),
                new FieldDefinition("marketplace_cut_percentage", "", TypeValue.BigUintTypeValue),
                new FieldDefinition("creator_royalties_percentage", "", TypeValue.BigUintTypeValue)
            });

            var decode = _sut.DecodeTopLevel(data, auction);
            var structValue = decode.ValueOf<StructValue>();

            var payment_token = structValue.GetStructField("payment_token").Value;
            Assert.That(payment_token, Is.Not.Null);
            Assert.That(payment_token, Is.TypeOf<StructValue>());
            Assert.That(payment_token.ValueOf<StructValue>().Fields.Length, Is.EqualTo(2));

            var esdtTokenStructValue = payment_token.ValueOf<StructValue>();
            var token_type = esdtTokenStructValue.GetStructField("token_type").Value;
            Assert.That(token_type.ValueOf<TokenIdentifierValue>().TokenName, Is.EqualTo("EGLD"));
            var nonce = esdtTokenStructValue.GetStructField("nonce").Value;
            Assert.That(nonce.ValueOf<NumericValue>().Number.IsZero, Is.True);

            Assert.That(structValue.GetStructField("min_bid"), Is.Not.Null);
            var min_bid = structValue.GetStructField("min_bid").Value;
            Assert.That(min_bid.ValueOf<NumericValue>().Number.ToString(), Is.EqualTo("100000000000000000"));

            Assert.That(structValue.GetStructField("max_bid"), Is.Not.Null);
            var max_bid = structValue.GetStructField("max_bid").Value;
            Assert.That(max_bid.ValueOf<NumericValue>().Number.ToString(), Is.EqualTo("10000000000000000000"));

            Assert.That(structValue.GetStructField("deadline"), Is.Not.Null);
            var deadline = structValue.GetStructField("deadline").Value;
            Assert.That(deadline.ValueOf<NumericValue>().Number.ToString(), Is.EqualTo("1621378452"));

            Assert.That(structValue.GetStructField("original_owner"), Is.Not.Null);
            var original_owner = structValue.GetStructField("original_owner").Value;
            Assert.That(original_owner.ValueOf<AddressValue>().Bech32,
                Is.EqualTo("erd1lkeja8knfjhkqzvrf3d9hmefxzt75wtf3vlg9m7ccugc8jmnrdpqy7yjeq"));

            Assert.That(structValue.GetStructField("current_bid"), Is.Not.Null);
            var current_bid = structValue.GetStructField("current_bid").Value;
            Assert.That(current_bid.ValueOf<NumericValue>().Number.ToString(), Is.EqualTo("0"));

            Assert.That(structValue.GetStructField("current_winner"), Is.Not.Null);
            var current_winner = structValue.GetStructField("current_winner").Value;
            Assert.That(current_winner.ValueOf<AddressValue>().Bech32, Is.EqualTo(AddressValue.Zero().Bech32));

            Assert.That(structValue.GetStructField("marketplace_cut_percentage"), Is.Not.Null);
            var marketplace_cut_percentage = structValue.GetStructField("marketplace_cut_percentage").Value;
            Assert.That(marketplace_cut_percentage.ValueOf<NumericValue>().Number.ToString(), Is.EqualTo("500"));

            Assert.That(structValue.GetStructField("creator_royalties_percentage"), Is.Not.Null);
            var creator_royalties_percentage = structValue.GetStructField("creator_royalties_percentage").Value;
            Assert.That(creator_royalties_percentage.ValueOf<NumericValue>().Number.ToString(), Is.EqualTo("0"));
        }
    }
}