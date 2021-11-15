using System;
using Erdcsharp.Domain;
using Erdcsharp.Domain.Helper;
using Erdcsharp.UnitTests.FakeData;
using NUnit.Framework;

namespace Erdcsharp.UnitTests.Domain
{
    [TestFixture(Category = "UnitTests")]
    public class WalletTests
    {
        private TestWalletFactory _walletFactory;

        [SetUp]
        public void Setup()
        {
            _walletFactory = new TestWalletFactory();
        }

        [Test]
        public void TestWalletFactory_Should_Create_Test_Wallets()
        {
            // Act
            var alice = _walletFactory.Alice.GetAccount().Address;
            var bob   = _walletFactory.Bob.GetAccount().Address;
            var carol = _walletFactory.Carol.GetAccount().Address;

            // Assert
            Assert.That(alice.Bech32, Is.EqualTo("erd1qyu5wthldzr8wx5c9ucg8kjagg0jfs53s8nr3zpz3hypefsdd8ssycr6th"));
            Assert.That(bob.Bech32, Is.EqualTo("erd1spyavw0956vq68xj8y4tenjpq2wd5a9p2c6j8gsz7ztyrnpxrruqzu66jx"));
            Assert.That(carol.Bech32, Is.EqualTo("erd1k2s324ww2g0yj38qn2ch2jwctdy8mnfxep94q9arncc6xecg3xaq6mjse8"));
        }

        [Test]
        public void GetPrivateKey_Should_ReturnCorrectKey()
        {
            // Arrange

            // Act
            var aliceSecretKey = _walletFactory.Alice.GetPrivateKey().ToHex();
            var bobSecretKey   = _walletFactory.Bob.GetPrivateKey().ToHex();
            var carolSecretKey = _walletFactory.Carol.GetPrivateKey().ToHex();

            // Assert
            Assert.That(aliceSecretKey, Is.EqualTo("413f42575f7f26fad3317a778771212fdb80245850981e48b58a4f25e344e8f9"));
            Assert.That(bobSecretKey, Is.EqualTo("b8ca6f8203fb4b545a8e83c5384da033c415db155b53fb5b8eba7ff5a039d639"));
            Assert.That(carolSecretKey, Is.EqualTo("e253a571ca153dc2aee845819f74bcc9773b0586edead15a94cb7235a5027436"));
        }

        [Test]
        public void Wallet_Should_Create_With_Secret_Keys()
        {
            // Arrange
            var secretKeyHex = "413f42575f7f26fad3317a778771212fdb80245850981e48b58a4f25e344e8f9";
            var buffer       = secretKeyHex.FromHex();

            // Act
            var wallet = new Wallet(buffer);

            // Assert
            Assert.That(wallet.GetPrivateKey().ToHex(), Is.EqualTo(secretKeyHex));
        }

        [TestCase("413f42575f7f26fad3317a778771212fdb80245850981e48b58a4f25e344e8f9",
                  "0139472eff6886771a982f3083da5d421f24c29181e63888228dc81ca60d69e1")]
        [TestCase("b8ca6f8203fb4b545a8e83c5384da033c415db155b53fb5b8eba7ff5a039d639",
                  "8049d639e5a6980d1cd2392abcce41029cda74a1563523a202f09641cc2618f8")]
        [TestCase("e253a571ca153dc2aee845819f74bcc9773b0586edead15a94cb7235a5027436",
                  "b2a11555ce521e4944e09ab17549d85b487dcd26c84b5017a39e31a3670889ba")]
        public void GetPublicKey_Should_Return_PublicKey(string secretKeyHex, string address)
        {
            // Arrange
            var buffer = secretKeyHex.FromHex();
            var wallet = new Wallet(buffer);

            // Act
            var publicKey = wallet.GetPublicKey();

            // Assert
            Assert.That(publicKey.ToHex(), Is.EqualTo(address));
        }

        [Test]
        public void Wallet_Should_Build_And_Decrypt_KeyFile()
        {
            // Arrange
            var wallet         = new Wallet("413f42575f7f26fad3317a778771212fdb80245850981e48b58a4f25e344e8f9");
            var randomPassword = Guid.NewGuid().ToString();

            // Act
            var keyFile = wallet.BuildKeyFile(randomPassword);

            // Assert
            Assert.That(keyFile, Is.Not.Null);
            Assert.That(keyFile.Address, Is.EqualTo("0139472eff6886771a982f3083da5d421f24c29181e63888228dc81ca60d69e1"));
            Assert.That(keyFile.Bech32, Is.EqualTo("erd1qyu5wthldzr8wx5c9ucg8kjagg0jfs53s8nr3zpz3hypefsdd8ssycr6th"));

            var newWallet = Wallet.DeriveFromKeyFile(keyFile, randomPassword);

            Assert.That(newWallet.GetPrivateKey().ToHex(), Is.EqualTo(wallet.GetPrivateKey().ToHex()));
            Assert.That(newWallet.GetPublicKey().ToHex(), Is.EqualTo(wallet.GetPublicKey().ToHex()));
        }

        [Test]
        public void DeriveFromKeyFile_Should_BuildWallet_From_JSONKeyFile()
        {
            // Arrange
            var keyFile =
                "{\"version\":4,\"id\":\"0dc10c02-b59b-4bac-9710-6b2cfa4284ba\",\"address\":\"0139472eff6886771a982f3083da5d421f24c29181e63888228dc81ca60d69e1\",\"bech32\":\"erd1qyu5wthldzr8wx5c9ucg8kjagg0jfs53s8nr3zpz3hypefsdd8ssycr6th\",\"crypto\":{\"ciphertext\":\"4c41ef6fdfd52c39b1585a875eb3c86d30a315642d0e35bb8205b6372c1882f135441099b11ff76345a6f3a930b5665aaf9f7325a32c8ccd60081c797aa2d538\",\"cipherparams\":{\"iv\":\"033182afaa1ebaafcde9ccc68a5eac31\"},\"cipher\":\"aes-128-ctr\",\"kdf\":\"scrypt\",\"kdfparams\":{\"dklen\":32,\"salt\":\"4903bd0e7880baa04fc4f886518ac5c672cdc745a6bd13dcec2b6c12e9bffe8d\",\"n\":4096,\"r\":8,\"p\":1},\"mac\":\"5b4a6f14ab74ba7ca23db6847e28447f0e6a7724ba9664cf425df707a84f5a8b\"}}";

            // Act
            var wallet = Wallet.DeriveFromKeyFile(KeyFile.From(keyFile), _walletFactory.Password);

            // Assert
            Assert.That(wallet.GetAccount().Address.Bech32,
                        Is.EqualTo("erd1qyu5wthldzr8wx5c9ucg8kjagg0jfs53s8nr3zpz3hypefsdd8ssycr6th"));
        }

        [TestCase("FakeData/KeyFiles/alice.json", "erd1qyu5wthldzr8wx5c9ucg8kjagg0jfs53s8nr3zpz3hypefsdd8ssycr6th")]
        [TestCase("FakeData/KeyFiles/bob.json", "erd1spyavw0956vq68xj8y4tenjpq2wd5a9p2c6j8gsz7ztyrnpxrruqzu66jx")]
        [TestCase("FakeData/KeyFiles/carol.json", "erd1k2s324ww2g0yj38qn2ch2jwctdy8mnfxep94q9arncc6xecg3xaq6mjse8")]
        public void DeriveFromKeyFile_Should_BuildWallet_From_KeyFilePath(string filePath, string bech32Address)
        {
            // Arrange

            // Act
            var wallet = Wallet.DeriveFromKeyFile(KeyFile.FromFilePath(filePath), _walletFactory.Password);

            // Assert
            Assert.That(wallet.GetAccount().Address.Bech32, Is.EqualTo(bech32Address));
        }

        [Test]
        public void Wallet_Should_Sign_Data()
        {
            // Arrange
            var wallet = new Wallet("1a927e2af5306a9bb2ea777f73e06ecc0ac9aaa72fb4ea3fecf659451394cccf");
            var message =
                "{\"nonce\":0,\"value\":\"0\",\"receiver\":\"erd1cux02zersde0l7hhklzhywcxk4u9n4py5tdxyx7vrvhnza2r4gmq4vw35r\",\"sender\":\"erd1l453hd0gt5gzdp7czpuall8ggt2dcv5zwmfdf3sd3lguxseux2fsmsgldz\",\"gasPrice\":1000000000,\"gasLimit\":50000,\"data\":\"Zm9v\",\"chainID\":\"1\",\"version\":1}";

            // Act
            var signature = wallet.Sign(message);

            // Assert
            Assert.That(signature, Is.EqualTo("b5fddb8c16fa7f6123cb32edc854f1e760a3eb62c6dc420b5a4c0473c58befd45b621b31a448c5b59e21428f2bc128c80d0ee1caa4f2bf05a12be857ad451b00"));
        }
    }
}
