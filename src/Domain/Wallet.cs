using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using dotnetstandard_bip32;
using dotnetstandard_bip39;
using Elrond.Dotnet.Sdk.Cryptography;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;

namespace Elrond.Dotnet.Sdk.Domain
{
    public class Wallet
    {
        private readonly byte[] _privateKey;
        private readonly byte[] _publicKey;
        private const string HdPrefix = "m/44'/508'/0'/0'";
        private static readonly RNGCryptoServiceProvider RngCsp = new RNGCryptoServiceProvider();

        public Wallet(string privateKeyHex)
            : this(Convert.FromHexString(privateKeyHex))
        {
        }

        public Wallet(byte[] privateKey)
        {
            var privateKeyParameters = new Ed25519PrivateKeyParameters(privateKey, 0);
            var publicKeyParameters = privateKeyParameters.GeneratePublicKey();
            _publicKey = publicKeyParameters.GetEncoded();
            _privateKey = privateKey;
        }

        public static Wallet DeriveFromMnemonic(string mnemonic, int accountIndex = 0)
        {
            try
            {
                var bip39 = new BIP39();
                var seedHex = bip39.MnemonicToSeedHex(mnemonic, "");

                var bip32 = new BIP32();
                var hdPath = $"{HdPrefix}/{accountIndex}'";
                var kv = bip32.DerivePath(hdPath, seedHex);
                var privateKey = kv.Key;
                return new Wallet(privateKey);
            }
            catch (Exception ex)
            {
                throw new Exception("CannotDeriveKeysException", ex);
            }
        }

        public static Wallet DeriveFromKeyFile(KeyFile keyFile, string secretPassword)
        {
            var saltBytes = Convert.FromHexString(keyFile.Crypto.Kdfparams.Salt);
            var kdParams = keyFile.Crypto.Kdfparams;
            var key = SCrypt.Generate(Encoding.UTF8.GetBytes(secretPassword), saltBytes, kdParams.N, kdParams.r, kdParams.p, kdParams.dklen);

            var rightPartOfKey = key.Skip(16).Take(16).ToArray();
            var leftPartOfKey = key.Take(16).ToArray();
            var mac = CreateSha256Signature(rightPartOfKey, keyFile.Crypto.Ciphertext);
            if (mac != keyFile.Crypto.Mac)
                throw new Exception("MAC mismatch, possibly wrong password");

            var decipher = EncryptAes128Ctr(Convert.FromHexString(keyFile.Crypto.Ciphertext),
                leftPartOfKey,
                Convert.FromHexString(keyFile.Crypto.Cipherparams.Iv));

            var privateKey = Convert.FromHexString(decipher);
            return new Wallet(privateKey);
        }

        public string Sign(string data)
        {
            return this.Sign(Encoding.UTF8.GetBytes(data));
        }

        public string Sign(byte[] data)
        {
            var parameters = new Ed25519PrivateKeyParameters(_privateKey, 0);
            var signer = new Ed25519Signer();
            signer.Init(true, parameters);
            signer.BlockUpdate(data, 0, data.Length);
            var signature = signer.GenerateSignature();

            return Convert.ToHexString(signature).ToLowerInvariant();
        }

        public byte[] GetPrivateKey()
        {
            return _privateKey;
        }

        public byte[] GetPublicKey()
        {
            return _publicKey;
        }

        private static string CreateSha256Signature(byte[] key, string targetText)
        {
            var data = Convert.FromHexString(targetText);
            byte[] mac;
            using (var hmac = new HMACSHA256(key))
            {
                mac = hmac.ComputeHash(data);
            }

            return Convert.ToHexString(mac).ToLowerInvariant();
        }

        private static string EncryptAes128Ctr(byte[] data, byte[] key, byte[] iv)
        {
            var output = AesCtr.Encrypt(key, iv, data);
            return Convert.ToHexString(output).ToLowerInvariant();
        }

        public KeyFile BuildKeyFile(string password)
        {
            var saltBytes = new byte[32];
            RngCsp.GetBytes(saltBytes);
            var ivBytes = new byte[16];
            RngCsp.GetBytes(ivBytes);

            var salt = Convert.ToHexString(saltBytes).ToLowerInvariant();
            var iv = Convert.ToHexString(ivBytes).ToLowerInvariant();
            var kdParams = new Kdfparams
            {
                dklen = 32,
                Salt = salt,
                N = 4096,
                r = 8,
                p = 1
            };

            var key = SCrypt.Generate(Encoding.UTF8.GetBytes(password), saltBytes, kdParams.N, kdParams.r, kdParams.p, kdParams.dklen);
            var leftPart = key.Take(16).ToArray();
            var rightPart = key.Skip(16).Take(16).ToArray();
            var cipher = EncryptAes128Ctr(_privateKey, leftPart, ivBytes);
            var mac = CreateSha256Signature(rightPart, cipher);

            var keyFile = new KeyFile
            {
                Version = 4,
                Id = Guid.NewGuid().ToString(),
                Address = Convert.ToHexString(_publicKey).ToLowerInvariant(),
                Bech32 = Bech32Engine.Encode("erd", _publicKey),
                Crypto = new Crypto
                {
                    Ciphertext = cipher,
                    Cipherparams = new Cipherparams()
                    {
                        Iv = iv
                    },
                    Cipher = "aes-128-ctr",
                    Kdf = "scrypt",
                    Kdfparams = kdParams,
                    Mac = mac
                }
            };
            return keyFile;
        }
    }
}