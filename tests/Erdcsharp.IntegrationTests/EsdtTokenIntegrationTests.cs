using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Numerics;
using System.Threading.Tasks;
using Erdcsharp.Configuration;
using Erdcsharp.Domain;
using Erdcsharp.Manager;
using Erdcsharp.Provider;
using Erdcsharp.UnitTests.FakeData;
using NUnit.Framework;

namespace Erdcsharp.IntegrationTests
{
    [TestFixture(Category = "LongRunning", Description = "EsdtTokenManager usage")]
    public class EsdtTokenIntegrationTests
    {
        private IElrondProvider   _provider;
        private IEsdtTokenManager _tokenManager;
        private TestWalletFactory _testWalletFactory;

        [SetUp]
        public void Setup()
        {
            _provider          = new ElrondProvider(new HttpClient(), new ElrondNetworkConfiguration(Network.TestNet));
            _tokenManager      = new EsdtTokenManager(_provider);
            _testWalletFactory = new TestWalletFactory();
        }

        [Test(Description = "Issue an ESDT FungibleESDT Token with a 1,000,000,000 initial supply")]
        public async Task Create_Esdt_Fungible_Token_For_Alice()
        {
            var wallet  = _testWalletFactory.Alice;
            var account = wallet.GetAccount();

            await account.Sync(_provider);

            var token         = Token.ESDT("MYTOKEN", "MTCK", 18);
            var initialSupply = TokenAmount.ESDT("1000000", token);
            var tokenIdentifier = await _tokenManager.IssueFungibleToken(
                                                                         wallet,
                                                                         token,
                                                                         initialSupply.Value);

            // ex : MTCK-0accc7
            Assert.That(tokenIdentifier.StartsWith(token.Ticker), "Token identifier should start with the Ticker");
            Assert.That(tokenIdentifier.Length, Is.EqualTo(token.Ticker.Length + 7),
                        "Token identifier should be ticker-[6 char]");

            await Transfer_One_ESDT_Token_From_Alice_To_Bob(tokenIdentifier);
        }

        [Test(Description = "Issue an ESDT NFT Token and create 1 token")]
        public async Task Create_Esdt_NFT_Token_For_Alice()
        {
            var wallet  = _testWalletFactory.Alice;
            var account = wallet.GetAccount();

            await account.Sync(_provider);

            var token = Token.ESDT_NFT("MYNFTTOKEN", "MYNFT");
            var tokenIdentifier = await _tokenManager.IssueNonFungibleToken(
                                                                            wallet,
                                                                            token.Name,
                                                                            token.Ticker);

            Assert.That(tokenIdentifier.StartsWith(token.Ticker), "Token identifier should start with the Ticker");
            Assert.That(tokenIdentifier.Length, Is.EqualTo(token.Ticker.Length + 7),
                        "Token identifier should be ticker-[6 char]");

            // Set the 'ESDTRoleNFTCreate' special role to alice
            await _tokenManager.SetSpecialRole(wallet, tokenIdentifier,
                                               Constants.EsdtNftSpecialRoles.EsdtRoleNftCreate);

            // Create the 'My foo NFT' NFT token
            var tokenId = await _tokenManager.CreateNftToken(
                                                             wallet,
                                                             tokenIdentifier,
                                                             BigInteger.One,
                                                             "My foo NFT",
                                                             450,
                                                             new Dictionary<string, string>(),
                                                             new[] {new Uri("https://www.foo.bar")});

            Assert.That(tokenId, Is.EqualTo(1));
        }

        [Test(Description = "Issue an ESDT SFT Token and create 5000 tokens")]
        public async Task Create_Esdt_SFT_Token_For_Alice()
        {
            var wallet  = _testWalletFactory.Alice;
            var account = wallet.GetAccount();

            await account.Sync(_provider);

            var token = Token.ESDT_NFT("MYSFTTOKEN", "MYSFT");

            var tokenIdentifier = await _tokenManager.IssueSemiFungibleToken(
                                                                             wallet,
                                                                             token.Name,
                                                                             token.Ticker);

            Assert.That(tokenIdentifier.StartsWith(token.Ticker), "Token identifier should start with the Ticker");
            Assert.That(tokenIdentifier.Length, Is.EqualTo(token.Ticker.Length + 7),
                        "Token identifier should be ticker-[6 char]");

            // Set the 'ESDTRoleNFTCreate' special role to alice
            await _tokenManager.SetSpecialRole(wallet, tokenIdentifier,
                                               Constants.EsdtSftSpecialRoles.EsdtRoleNftCreate,
                                               Constants.EsdtSftSpecialRoles.EsdtRoleNftAddQuantity);

            // Create the 'My foo NFT' NFT token
            var tokenId = await _tokenManager.CreateNftToken(
                                                             wallet,
                                                             tokenIdentifier,
                                                             5000,
                                                             "My foo SFT",
                                                             450,
                                                             new Dictionary<string, string>()
                                                             {
                                                                 {"artist", "my artist"},
                                                                 {"foo", "bar"}
                                                             },
                                                             new[] {new Uri("https://www.foo.bar")});

            Assert.That(tokenId, Is.EqualTo(1));
        }

        [Test(Description = "Get a collection of ESDT Tokens for alice")]
        public async Task Query_Esdt_Tokens_For_Address()
        {
            var alice  = _testWalletFactory.Alice.GetAccount().Address;
            var tokens = await _tokenManager.GetEsdtTokens(alice);

            Assert.That(tokens, Is.Not.Null);
            Assert.Pass();
        }

        private async Task Transfer_One_ESDT_Token_From_Alice_To_Bob(string tokenIdentifier)
        {
            await Task.Delay(5000);
            var alice        = _testWalletFactory.Alice;
            var aliceAccount = alice.GetAccount();

            var bob               = _testWalletFactory.Bob.GetAccount().Address;
            var tokenInBobAccount = await _tokenManager.GetEsdtFungibleToken(bob, tokenIdentifier);
            Assert.That(tokenInBobAccount.Balance.IsZero, Is.True, "Bob should not have any token at this time");

            var tokenInAliceAccount = await _tokenManager.GetEsdtFungibleToken(aliceAccount.Address, tokenIdentifier);
            Assert.That(tokenInAliceAccount.Balance > 1, Is.True, "Alice should have at least 1 token");
            await _tokenManager.TransferEsdtToken(alice, tokenInAliceAccount, bob, BigInteger.One);

            tokenInBobAccount = await _tokenManager.GetEsdtFungibleToken(bob, tokenIdentifier);
            Assert.That(tokenInBobAccount.Balance.IsOne, Is.True, "Bob should have only one token");
        }
    }
}
