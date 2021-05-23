﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Elrond.Dotnet.Sdk.Domain;
using Elrond.Dotnet.Sdk.Domain.Values;
using NUnit.Framework;

namespace Elrond_sdk.dotnet.tests.Domain
{
    public class ESDTTokenTests
    {
        private readonly Account _account;
        private readonly AddressValue _receiver;
        private readonly Constants _constants;

        public ESDTTokenTests()
        {
            _account = new Account(
                AddressValue.FromBech32("erd1sg4u62lzvgkeu4grnlwn7h2s92rqf8a64z48pl9c7us37ajv9u8qj9w8xg"));
            _receiver = AddressValue.FromBech32("erd1spyavw0956vq68xj8y4tenjpq2wd5a9p2c6j8gsz7ztyrnpxrruqzu66jx");
            _constants = Constants.New();
            _constants.ChainId = "1";
            _constants.GasPerDataByte = 1500;
            _constants.MinGasPrice = 1000000000;
            _constants.MinGasLimit = 50000;
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void IssueNonFungibleTokenTransactionRequest()
        {
            // Arrange
            const string tokenName = "Token name";
            const string tokenTicker = "TKN";

            // Act
            var transaction = ESDTTokenTransactionRequest.IssueNonFungibleTokenTransactionRequest(
                _constants,
                _account,
                tokenName,
                tokenTicker);

            // Assert
            Assert.That(transaction, Is.Not.Null);
            Assert.That(transaction.Receiver.Bech32,
                Is.EqualTo("erd1qqqqqqqqqqqqqqqpqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqzllls8a5w6u"));
            Assert.That(transaction.Sender, Is.EqualTo(_account.Address));
            Assert.That(Encoding.UTF8.GetString(Convert.FromBase64String(transaction.Data)),
                Is.EqualTo("issueNonFungible@546F6B656E206E616D65@544B4E"));
            Assert.That(transaction.GasLimit.Value, Is.EqualTo(60000000));
            Assert.That(transaction.Value.Number.ToString(), Is.EqualTo(50000000000000000.ToString()));
        }

        [Test]
        public void IssueSemiFungibleTokenTransactionRequest()
        {
            // Arrange
            const string tokenName = "Token name";
            const string tokenTicker = "TKN";

            // Act
            var transaction = ESDTTokenTransactionRequest.IssueSemiFungibleTokenTransactionRequest(
                _constants,
                _account,
                tokenName,
                tokenTicker);

            // Assert
            Assert.That(transaction, Is.Not.Null);
            Assert.That(transaction.Receiver.Bech32,
                Is.EqualTo("erd1qqqqqqqqqqqqqqqpqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqzllls8a5w6u"));
            Assert.That(transaction.Sender, Is.EqualTo(_account.Address));
            Assert.That(Encoding.UTF8.GetString(Convert.FromBase64String(transaction.Data)),
                Is.EqualTo("issueSemiFungible@546F6B656E206E616D65@544B4E"));
            Assert.That(transaction.GasLimit.Value, Is.EqualTo(60000000));
            Assert.That(transaction.Value.Number.ToString(), Is.EqualTo(50000000000000000.ToString()));
        }

        [Test]
        public void IssueESDTTransactionRequest()
        {
            // Arrange
            const string tokenName = "AliceTokens";
            const string tokenTicker = "ALC";

            // Act
            var transaction = ESDTTokenTransactionRequest.IssueESDTTransactionRequest(
                _constants,
                _account,
                tokenName,
                tokenTicker,
                new BigInteger(4091000000),
                6);

            // Assert
            Assert.That(transaction, Is.Not.Null);
            Assert.That(transaction.Receiver.Bech32,
                Is.EqualTo("erd1qqqqqqqqqqqqqqqpqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqzllls8a5w6u"));
            Assert.That(transaction.Sender, Is.EqualTo(_account.Address));
            Assert.That(Encoding.UTF8.GetString(Convert.FromBase64String(transaction.Data)),
                Is.EqualTo("issue@416C696365546F6B656E73@414C43@F3D7B4C0@06"));
            Assert.That(transaction.GasLimit.Value, Is.EqualTo(60000000));
            Assert.That(transaction.Value.Number.ToString(), Is.EqualTo(50000000000000000.ToString()));
        }

        [Test]
        public void TransferESDTTransactionRequest()
        {
            // Arrange
            const string tokenIdentifier = "ALC-6258d2";

            // Act
            var transaction = ESDTTokenTransactionRequest.TransferESDTTransactionRequest(
                _constants,
                _account,
                _receiver,
                tokenIdentifier,
                12);

            // Assert
            Assert.That(transaction, Is.Not.Null);
            Assert.That(transaction.Receiver.Bech32, Is.EqualTo(_receiver.Bech32));
            Assert.That(transaction.Sender, Is.EqualTo(_account.Address));

            Assert.That(Encoding.UTF8.GetString(Convert.FromBase64String(transaction.Data)),
                Is.EqualTo("ESDTTransfer@414C432D363235386432@0C"));
            Assert.That(transaction.GasLimit.Value, Is.EqualTo(500000));
            Assert.That(transaction.Value.Number.ToString(), Is.EqualTo(0.ToString()));
        }

        [Test]
        public void TransferESDTNFTTransactionRequest()
        {
            // Arrange
            const string tokenIdentifier = "ALC-6258d2";

            // Act
            var transaction = ESDTTokenTransactionRequest.TransferESDTNFTTransactionRequest(
                _constants,
                _account,
                _receiver,
                tokenIdentifier,
                12,
                1);

            // Assert
            Assert.That(transaction, Is.Not.Null);
            Assert.That(transaction.Receiver, Is.EqualTo(_account.Address));
            Assert.That(transaction.Sender, Is.EqualTo(_account.Address));

            Assert.That(Encoding.UTF8.GetString(Convert.FromBase64String(transaction.Data)),
                Is.EqualTo(
                    "ESDTNFTTransfer@414C432D363235386432@0C@01@8049D639E5A6980D1CD2392ABCCE41029CDA74A1563523A202F09641CC2618F8"));
            Assert.That(transaction.GasLimit.Value, Is.EqualTo(500000));
            Assert.That(transaction.Value.Number.ToString(), Is.EqualTo(0.ToString()));
        }

        [Test]
        public void SetSpecialRoleTransactionRequest()
        {
            // Arrange
            const string tokenIdentifier = "ALC-6258d2";

            // Act
            var transaction = ESDTTokenTransactionRequest.SetSpecialRoleTransactionRequest(
                _constants,
                _account,
                _receiver,
                tokenIdentifier,
                ESDTTokenTransactionRequest.NFTRoles.ESDTRoleNFTCreate,
                ESDTTokenTransactionRequest.NFTRoles.ESDTRoleNFTBurn);

            // Assert
            Assert.That(transaction, Is.Not.Null);
            Assert.That(transaction.Receiver.Bech32,
                Is.EqualTo("erd1qqqqqqqqqqqqqqqpqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqzllls8a5w6u"));
            Assert.That(transaction.Sender, Is.EqualTo(_account.Address));

            var hex = Encoding.UTF8.GetString(Convert.FromBase64String(transaction.Data));
            Assert.That(hex,
                Is.EqualTo(
                    $"setSpecialRole@414C432D363235386432@{_receiver.Hex}@45534454526F6C654E4654437265617465@45534454526F6C654E46544275726E"));
            Assert.That(transaction.GasLimit.Value, Is.EqualTo(60000000));
            Assert.That(transaction.Value.Number.ToString(), Is.EqualTo(0.ToString()));
        }

        [Test]
        public void CreateESDTNFTTokenTransactionRequest()
        {
            // Arrange
            const string tokenIdentifier = "ALC-6258d2";

            // Act
            var transaction = ESDTTokenTransactionRequest.CreateESDTNFTTokenTransactionRequest(
                _constants,
                _account,
                tokenIdentifier,
                "Beautiful song",
                7500,
                "",
                new Dictionary<string, string>
                {
                    {"Artist", "Famous artist"},
                    {"Duration", "03.17"},
                },
                new[] {new Uri("https://wwww.to_decentralized_storage/song.mp3")});

            // Assert
            Assert.That(transaction, Is.Not.Null);
            Assert.That(transaction.Receiver.Bech32, Is.EqualTo(_account.Address.Bech32));
            Assert.That(transaction.Sender, Is.EqualTo(_account.Address));

            var hex = Encoding.UTF8.GetString(Convert.FromBase64String(transaction.Data));
            Assert.That(hex,
                Is.EqualTo(
                    $"ESDTNFTCreate@414C432D363235386432@01@42656175746966756C20736F6E67@1D4C@@4172746973743A46616D6F7573206172746973743B4475726174696F6E3A30332E3137@55524C5F746F5F646563656E7472616C697A65645F73746F726167652F736F6E672E6D7033"));
            Assert.That(transaction.GasLimit.Value, Is.EqualTo(9927000));
            Assert.That(transaction.Value.Number.ToString(), Is.EqualTo(0.ToString()));
        }
    }
}