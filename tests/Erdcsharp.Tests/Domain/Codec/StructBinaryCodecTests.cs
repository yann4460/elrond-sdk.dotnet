using System;
using System.Numerics;
using Erdcsharp.Domain;
using Erdcsharp.Domain.Codec;
using Erdcsharp.Domain.Values;
using NUnit.Framework;

namespace Erdcsharp.UnitTests.Domain.Codec
{
    [TestFixture(Category = "UnitTests")]
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
            var structField     = new StructField("azeazeaze", NumericValue.U16Value(12));
            var type            = TypeValue.StructValue("azeae", new[] {fieldDefinition});

            // Act
            var actual    = _sut.EncodeNested(new StructValue(type, new[] {structField}));
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
                new StructField("ticket_price", NumericValue.TokenAmount(TokenAmount.EGLD("10"))),
                new StructField("tickets_left", NumericValue.U32Value(0)),
                new StructField("deadline", NumericValue.U64Value(0x000000005fc2b9db)),
                new StructField("max_entries_per_user", NumericValue.U32Value(0xffffffff)),
                new StructField("prize_distribution", BytesValue.FromBuffer(new byte[] {0x64})),
                new StructField("current_ticket_number", NumericValue.U32Value(9472)),
                new StructField("prize_pool", NumericValue.BigUintValue(BigInteger.Parse("94720000000000000000000"))),
            });

            // Act
            var actual    = _sut.EncodeNested(structValue);
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

            var decode      = _sut.DecodeTopLevel(data, auction);
            var structValue = decode.ValueOf<StructValue>();

            var payment_token = structValue.GetStructField("payment_token").Value;
            Assert.That(payment_token, Is.Not.Null);
            Assert.That(payment_token, Is.TypeOf<StructValue>());
            Assert.That(payment_token.ValueOf<StructValue>().Fields.Length, Is.EqualTo(2));

            var esdtTokenStructValue = payment_token.ValueOf<StructValue>();
            var token_type           = esdtTokenStructValue.GetStructField("token_type").Value;
            Assert.That(token_type.ValueOf<TokenIdentifierValue>().Value, Is.EqualTo(Constants.EGLD));
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
            Assert.That(original_owner.ValueOf<Address>().Bech32,
                        Is.EqualTo("erd1lkeja8knfjhkqzvrf3d9hmefxzt75wtf3vlg9m7ccugc8jmnrdpqy7yjeq"));

            Assert.That(structValue.GetStructField("current_bid"), Is.Not.Null);
            var current_bid = structValue.GetStructField("current_bid").Value;
            Assert.That(current_bid.ValueOf<NumericValue>().Number.ToString(), Is.EqualTo("0"));

            Assert.That(structValue.GetStructField("current_winner"), Is.Not.Null);
            var current_winner = structValue.GetStructField("current_winner").Value;
            Assert.That(current_winner.ValueOf<Address>().Bech32, Is.EqualTo(Address.Zero().Bech32));

            Assert.That(structValue.GetStructField("marketplace_cut_percentage"), Is.Not.Null);
            var marketplace_cut_percentage = structValue.GetStructField("marketplace_cut_percentage").Value;
            Assert.That(marketplace_cut_percentage.ValueOf<NumericValue>().Number.ToString(), Is.EqualTo("500"));

            Assert.That(structValue.GetStructField("creator_royalties_percentage"), Is.Not.Null);
            var creator_royalties_percentage = structValue.GetStructField("creator_royalties_percentage").Value;
            Assert.That(creator_royalties_percentage.ValueOf<NumericValue>().Number.ToString(), Is.EqualTo("0"));
        }

        [Test]
        public void Decode_Complex_Value()
        {
            // Arrange
            var esdtToken = TypeValue.StructValue("EsdtToken", new[]
            {
                new FieldDefinition("token_type", "", TypeValue.TokenIdentifierValue),
                new FieldDefinition("nonce", "", TypeValue.U64TypeValue)
            });

            // Act
            var actual = _sut.EncodeNested(new StructValue(esdtToken,
                                                           new[]
                                                           {
                                                               new StructField("token", TokenIdentifierValue.From("FRAMEIT-556945")),
                                                               new StructField("nonce", new NumericValue(TypeValue.U64TypeValue, new BigInteger(1)))
                                                           }));

            var hexEncode = Convert.ToHexString(actual);

            //0000000E4652414D4549542D3535363934350000000000000001
            var hex  = "0000000e4652414d4549542d3535363934350000000000000001";
            var data = Convert.FromHexString(hex);

            // Arrange


            var decode = _sut.DecodeNested(data, esdtToken);
        }


        [Test]
        public void Decode_Complex_Value_From_Vm()
        {
            var data =
                "AAAADkZSQU1FSVQtNTU2OTQ1AAAAAAAAAEEAAAABAQEAAAAERUdMRAAAAAAAAAAAAAAABwONfqTGgAABAAAACBTREg17FgAAAAAAAGHDcy4AAAAAYl2IiAZDfyQkrs85UMPwiblXPfL5+adcSHpoU40nC0p6qK2NAAAABwONfqTGgAAMV1PPzBMQw+vMERf0jm82fvEIFkxlb0GCpkcQVfTtEwAAAAFkAAAAAgPo";
            var bytes = Convert.FromBase64String(data);

            var esdtToken = TypeValue.StructValue("EsdtToken",
                                                  new[]
                                                  {
                                                      new FieldDefinition("token_type", "", TypeValue.TokenIdentifierValue),
                                                      new FieldDefinition("nonce", "", TypeValue.U64TypeValue)
                                                  });

            var auctionType = TypeValue.EnumValue("AuctionType",
                                                  new[]
                                                  {
                                                      new EnumVariantDefinition("None", 0),
                                                      new EnumVariantDefinition("Nft", 1),
                                                      new EnumVariantDefinition("SftAll", 2),
                                                      new EnumVariantDefinition("SftOnePerPayment", 3)
                                                  });

            var auction = TypeValue.StructValue("Auction",
                                                new[]
                                                {
                                                    new FieldDefinition("auctioned_token", "", esdtToken),
                                                    new FieldDefinition("nr_auctioned_tokens", "", TypeValue.BigUintTypeValue),
                                                    new FieldDefinition("auction_type", "", auctionType),
                                                    new FieldDefinition("payment_token", "", esdtToken),
                                                    new FieldDefinition("min_bid", "", TypeValue.BigUintTypeValue),
                                                    new FieldDefinition("max_bid", "", TypeValue.OptionValue(TypeValue.BigUintTypeValue)),
                                                    new FieldDefinition("start_time", "", TypeValue.U64TypeValue), new FieldDefinition("deadline", "", TypeValue.U64TypeValue),
                                                    new FieldDefinition("original_owner", "", TypeValue.AddressValue),
                                                    new FieldDefinition("current_bid", "", TypeValue.BigUintTypeValue),
                                                    new FieldDefinition("current_winner", "", TypeValue.AddressValue),
                                                    new FieldDefinition("marketplace_cut_percentage", "", TypeValue.BigUintTypeValue),
                                                    new FieldDefinition("creator_royalties_percentage", "", TypeValue.BigUintTypeValue)
                                                });

            var structBinaryCodec = new StructBinaryCodec(new BinaryCodec());
            var decoded           = structBinaryCodec.DecodeTopLevel(bytes, auction);
        }
    }
}
