namespace Elrond.Dotnet.Sdk.Domain
{
    public class KeyFile
    {
        public int Version { get; set; }
        public string Id { get; set; }
        public string Address { get; set; }
        public string Bech32 { get; set; }
        public Crypto Crypto { get; set; }
    }

    public class Crypto
    {
        public string Ciphertext { get; set; }
        public Cipherparams Cipherparams { get; set; }
        public string Cipher { get; set; }
        public string Kdf { get; set; }
        public Kdfparams Kdfparams { get; set; }
        public string Mac { get; set; }
    }

    public class Cipherparams
    {
        public string Iv { get; set; }
    }

    public class Kdfparams
    {
        public int dklen { get; set; }
        public string Salt { get; set; }
        public int N { get; set; }
        public int r { get; set; }
        public int p { get; set; }
    }
}