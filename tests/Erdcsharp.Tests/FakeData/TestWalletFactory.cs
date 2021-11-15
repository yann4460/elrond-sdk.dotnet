using Erdcsharp.Domain;

namespace Erdcsharp.UnitTests.FakeData
{
    public class TestWalletFactory
    {
        public string Mnemonic { get; }
        public string Password { get; }

        public TestWalletFactory()
        {
            Mnemonic = "moral volcano peasant pass circle pen over picture flat shop clap goat never lyrics gather prepare woman film husband gravity behind test tiger improve";
            Password = "password";
            Alice    = Wallet.DeriveFromMnemonic(Mnemonic, 0);
            Bob      = Wallet.DeriveFromMnemonic(Mnemonic, 1);
            Carol    = Wallet.DeriveFromMnemonic(Mnemonic, 2);
        }

        public Wallet Bob { get; set; }

        public Wallet Carol { get; set; }

        public Wallet Alice { get; set; }
    }

    public static class TestData
    {
        public const string AliceBech32 = "erd1qyu5wthldzr8wx5c9ucg8kjagg0jfs53s8nr3zpz3hypefsdd8ssycr6th";
        public const string AliceHex    = "0139472eff6886771a982f3083da5d421f24c29181e63888228dc81ca60d69e1";

        public const string BobBech32 = "erd1spyavw0956vq68xj8y4tenjpq2wd5a9p2c6j8gsz7ztyrnpxrruqzu66jx";
        public const string BobHex    = "8049d639e5a6980d1cd2392abcce41029cda74a1563523a202f09641cc2618f8";

        public const string CarolBech32 = "erd1k2s324ww2g0yj38qn2ch2jwctdy8mnfxep94q9arncc6xecg3xaq6mjse8";
        public const string CarolHex    = "b2a11555ce521e4944e09ab17549d85b487dcd26c84b5017a39e31a3670889ba";
    }
}
