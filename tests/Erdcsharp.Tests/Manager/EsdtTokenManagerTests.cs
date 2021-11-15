using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Erdcsharp.Domain;
using Erdcsharp.Domain.Helper;
using Erdcsharp.Manager;
using Erdcsharp.Provider.Dtos;
using Erdcsharp.UnitTests.FakeData;
using Moq;
using NUnit.Framework;

namespace Erdcsharp.UnitTests.Manager
{
    [TestFixture(Category = "UnitTests")]
    public class EsdtTokenManagerTests
    {
        private ElrondGatewayMockProvider _elrondGatewayMockProvider;
        private IEsdtTokenManager         _sut;
        private TestWalletFactory         _testWalletFactory;
        private string                    _txHash;

        [SetUp]
        public void Setup()
        {
            _elrondGatewayMockProvider = new ElrondGatewayMockProvider();
            _testWalletFactory         = new TestWalletFactory();
            _txHash                    = Guid.NewGuid().ToString();
            _elrondGatewayMockProvider.MockProvider.Setup(s => s.SendTransaction(It.IsAny<TransactionRequestDto>()))
                                      .ReturnsAsync(
                                                    new CreateTransactionResponseDataDto()
                                                    {
                                                        TxHash = _txHash
                                                    });
            _sut = new EsdtTokenManager(_elrondGatewayMockProvider.MockProvider.Object, NetworkConfig.New());
        }

        [Test]
        public async Task IssueFungibleToken_Should_Return_TokenIdentifier()
        {
            // Arrange
            var alice = _testWalletFactory.Alice;
            var token = Token.ESDT("TOKEN", "TKN", 18);

            _elrondGatewayMockProvider.SetTransactionDetailResult(scResult: new[]
            {
                new SmartContractResultDto
                {
                    Nonce = 0,
                    Data  = $"ESDTTransfer@{Converter.ToHexString("TK-651452")}@d3c21bcecceda1000000"
                },
                new SmartContractResultDto
                {
                    Nonce = 150,
                    Data  = "@ok"
                }
            });

            // Act
            var tokenIdentifier = await _sut.IssueFungibleToken(alice, token, 1000000000);

            // Assert
            Assert.That(tokenIdentifier, Is.EqualTo("TK-651452"));
        }

        [Test]
        public async Task SetSpecialRole_Should_Passed()
        {
            // Arrange
            var          alice           = _testWalletFactory.Alice;
            const string tokenIdentifier = "TK-651452";

            _elrondGatewayMockProvider.SetTransactionDetailResult();

            // Act
            await _sut.SetSpecialRole(alice, tokenIdentifier, Constants.EsdtNftSpecialRoles.EsdtRoleNftCreate);

            // Assert
            var expectedData =
                $"setSpecialRole@{Converter.ToHexString(tokenIdentifier)}@{alice.GetAccount().Address.Hex}@{Converter.ToHexString(Constants.EsdtNftSpecialRoles.EsdtRoleNftCreate)}";
            var expectedEncodedData = Converter.ToBase64String(expectedData);

            _elrondGatewayMockProvider.MockProvider.Verify(
                                                           s => s.SendTransaction(It.Is<TransactionRequestDto>(t =>
                                                                                                                   t.Data == expectedEncodedData &&
                                                                                                                   t.Receiver == Constants.SmartContractAddress.EsdtSmartContract &&
                                                                                                                   t.Value == "0"
                                                                                                              )), Times.Once);

            Assert.Pass();
        }

        [Test]
        public async Task CreateNftToken_Should_Return_TokenNonce()
        {
            // Arrange
            var          alice           = _testWalletFactory.Alice;
            const string tokenIdentifier = "TK-651452";

            _elrondGatewayMockProvider.SetTransactionDetailResult(scResult: new[]
            {
                new SmartContractResultDto
                {
                    Nonce = 0,
                    Data  = "@ok@01"
                }
            });

            // Act
            var tokenId = await _sut.CreateNftToken(
                                                    alice,
                                                    tokenIdentifier,
                                                    BigInteger.One,
                                                    "My token name",
                                                    550,
                                                    new Dictionary<string, string>(),
                                                    new[] {new Uri("https://foo.bar")}
                                                   );

            // Assert
            var expectedData =
                $"ESDTNFTCreate@544B2D363531343532@01@4D7920746F6B656E206E616D65@0226@@@68747470733A2F2F666F6F2E6261722F";
            var expectedEncodedData = Converter.ToBase64String(expectedData);
            _elrondGatewayMockProvider.MockProvider.Verify(
                                                           s => s.SendTransaction(It.Is<TransactionRequestDto>(t =>
                                                                                                                   t.Data == expectedEncodedData &&
                                                                                                                   t.Receiver == alice.GetAccount().Address.Bech32 &&
                                                                                                                   t.Value == "0"
                                                                                                              )), Times.Once);

            // Assert
            Assert.That(tokenId, Is.EqualTo(1));
        }
    }
}
