using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using dotnetstandard_bip39;
using Erdcsharp.Cryptography;
using Erdcsharp.Domain.Helper;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;

namespace Erdcsharp.Domain
{
    public class Wallet
    {
        private readonly        byte[]                   _privateKey;
        private readonly        byte[]                   _publicKey;
        private const           string                   HdPrefix = "m/44'/508'/0'/0'";
        private static readonly RNGCryptoServiceProvider RngCsp   = new RNGCryptoServiceProvider();

        public Wallet(string privateKeyHex)
            : this(Converter.FromHexString(privateKeyHex))
        {
        }

        /// <summary>
        /// Get the account wallet
        /// </summary>
        /// <returns>Account</returns>
        public Account GetAccount()
        {
            return new Account(Address.FromBytes(_publicKey));
        }

        /// <summary>
        /// Build a wallet
        /// </summary>
        /// <param name="privateKey">The private key</param>
        public Wallet(byte[] privateKey)
        {
            var privateKeyParameters = new Ed25519PrivateKeyParameters(privateKey, 0);
            var publicKeyParameters  = privateKeyParameters.GeneratePublicKey();
            _publicKey  = publicKeyParameters.GetEncoded();
            _privateKey = privateKey;
        }

        /// <summary>
        /// Derive a wallet from Mnemonic phrase
        /// </summary>
        /// <param name="mnemonic">The mnemonic phrase</param>
        /// <param name="accountIndex">The account index, default 0</param>
        /// <returns>Wallet</returns>
        public static Wallet DeriveFromMnemonic(string mnemonic, int accountIndex = 0)
        {
            try
            {
                var bip39   = new BIP39();
                var seedHex = bip39.MnemonicToSeedHex(mnemonic, "");

                var hdPath     = $"{HdPrefix}/{accountIndex}'";
                var kv         = DerivePath(hdPath, seedHex);
                var privateKey = kv.Key;
                return new Wallet(privateKey);
            }
            catch (Exception ex)
            {
                throw new Exception("CannotDeriveKeysException", ex);
            }
        }

        /// <summary>
        /// Derive a wallet from a KeyFile and a password
        /// </summary>
        /// <param name="keyFile">The KeyFile</param>
        /// <param name="secretPassword">The password</param>
        /// <returns>Wallet</returns>
        public static Wallet DeriveFromKeyFile(KeyFile keyFile, string secretPassword)
        {
            var saltBytes = Converter.FromHexString(keyFile.Crypto.Kdfparams.Salt);
            var kdParams  = keyFile.Crypto.Kdfparams;
            var key = SCrypt.Generate(Encoding.UTF8.GetBytes(secretPassword), saltBytes, kdParams.N, kdParams.r,
                                      kdParams.p, kdParams.dklen);

            var rightPartOfKey = key.Skip(16).Take(16).ToArray();
            var leftPartOfKey  = key.Take(16).ToArray();
            var mac            = CreateSha256Signature(rightPartOfKey, keyFile.Crypto.Ciphertext);
            if (mac != keyFile.Crypto.Mac)
                throw new Exception("MAC mismatch, possibly wrong password");

            var decipher = EncryptAes128Ctr(Converter.FromHexString(keyFile.Crypto.Ciphertext),
                                            leftPartOfKey,
                                            Converter.FromHexString(keyFile.Crypto.Cipherparams.Iv));

            var privateKey = Converter.FromHexString(decipher);
            return new Wallet(privateKey);
        }

        /// <summary>
        /// Sign data string with the wallet
        /// </summary>
        /// <param name="data">The data to signed</param>
        /// <returns>Signature</returns>
        public string Sign(string data)
        {
            return Sign(Encoding.UTF8.GetBytes(data));
        }

        /// <summary>
        /// Sign data with the wallet
        /// </summary>
        /// <param name="data">The data to signed</param>
        /// <returns>Signature</returns>
        public string Sign(byte[] data)
        {
            var parameters = new Ed25519PrivateKeyParameters(_privateKey, 0);
            var signer     = new Ed25519Signer();
            signer.Init(true, parameters);
            signer.BlockUpdate(data, 0, data.Length);
            var signature = signer.GenerateSignature();

            return Converter.ToHexString(signature).ToLowerInvariant();
        }

        /// <summary>
        /// The private key. Do not share
        /// </summary>
        /// <returns>Private key</returns>
        public byte[] GetPrivateKey()
        {
            return _privateKey;
        }

        /// <summary>
        /// The public key
        /// </summary>
        /// <returns>Public key</returns>
        public byte[] GetPublicKey()
        {
            return _publicKey;
        }

        /// <summary>
        /// Build a key file
        /// </summary>
        /// <param name="password">Password</param>
        /// <returns>KeyFile</returns>
        public KeyFile BuildKeyFile(string password)
        {
            var saltBytes = new byte[32];
            RngCsp.GetBytes(saltBytes);
            var ivBytes = new byte[16];
            RngCsp.GetBytes(ivBytes);

            var salt = Converter.ToHexString(saltBytes).ToLowerInvariant();
            var iv   = Converter.ToHexString(ivBytes).ToLowerInvariant();
            var kdParams = new Crypto.KdfSructure
            {
                dklen = 32,
                Salt  = salt,
                N     = 4096,
                r     = 8,
                p     = 1
            };

            var encodedPassword = Encoding.UTF8.GetBytes(password);
            var key             = SCrypt.Generate(encodedPassword, saltBytes, kdParams.N, kdParams.r, kdParams.p, kdParams.dklen);
            var leftPart        = key.Take(16).ToArray();
            var rightPart       = key.Skip(16).Take(16).ToArray();
            var cipher          = EncryptAes128Ctr(_privateKey, leftPart, ivBytes);
            var mac             = CreateSha256Signature(rightPart, cipher);

            var keyFile = new KeyFile
            {
                Version = 4,
                Id      = Guid.NewGuid().ToString(),
                Address = Converter.ToHexString(_publicKey).ToLowerInvariant(),
                Bech32  = Bech32Engine.Encode(Constants.Hrp, _publicKey),
                Crypto = new Crypto
                {
                    Ciphertext   = cipher,
                    Cipherparams = new Crypto.CipherStructure() {Iv = iv},
                    Cipher       = "aes-128-ctr",
                    Kdf          = "scrypt",
                    Kdfparams    = kdParams,
                    Mac          = mac
                }
            };
            return keyFile;
        }

        private static string CreateSha256Signature(byte[] key, string targetText)
        {
            var    data = Converter.FromHexString(targetText);
            byte[] mac;
            using (var hmac = new HMACSHA256(key))
            {
                mac = hmac.ComputeHash(data);
            }

            return Converter.ToHexString(mac).ToLowerInvariant();
        }

        private static string EncryptAes128Ctr(byte[] data, byte[] key, byte[] iv)
        {
            var output = AesCtr.Encrypt(key, iv, data);
            return Converter.ToHexString(output).ToLowerInvariant();
        }

        private static (byte[] Key, byte[] ChainCode) DerivePath(string path, string seed)
        {
            var masterKeyFromSeed = GetMasterKeyFromSeed(seed);
            var segments = path
                          .Split('/')
                          .Skip(1)
                          .Select(a => a.Replace("'", ""))
                          .Select(a => Convert.ToUInt32(a, 10));

            var results = segments
               .Aggregate(masterKeyFromSeed, (mks, next) => GetChildKeyDerivation(mks.Key, mks.ChainCode, next + 0x80000000));

            return results;
        }

        private static (byte[] Key, byte[] ChainCode) GetMasterKeyFromSeed(string seed)
        {
            using (var hmacSha512 = new HMACSHA512(Encoding.UTF8.GetBytes("ed25519 seed")))
            {
                var i = hmacSha512.ComputeHash(Converter.FromHexString(seed));

                var il = i.Slice(0, 32);
                var ir = i.Slice(32);

                return (Key: il, ChainCode: ir);
            }
        }

        private static (byte[] Key, byte[] ChainCode) GetChildKeyDerivation(byte[] key, byte[] chainCode, uint index)
        {
            var buffer = new BigEndianBuffer();

            buffer.Write(new byte[] {0});
            buffer.Write(key);
            buffer.WriteUInt(index);

            using (var hmacSha512 = new HMACSHA512(chainCode))
            {
                var i = hmacSha512.ComputeHash(buffer.ToArray());

                var il = i.Slice(0, 32);
                var ir = i.Slice(32);

                return (Key: il, ChainCode: ir);
            }
        }
    }
}
