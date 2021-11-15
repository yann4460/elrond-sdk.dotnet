using System.IO;
using Erdcsharp.Domain.Helper;

namespace Erdcsharp.Domain
{
    public class KeyFile
    {
        public int    Version { get; set; }
        public string Id      { get; set; }
        public string Address { get; set; }
        public string Bech32  { get; set; }
        public Crypto Crypto  { get; set; }

        /// <summary>
        /// Load a KeyFile object from a json string
        /// </summary>
        /// <param name="json">JSON String</param>
        /// <returns>KeyFile object</returns>
        public static KeyFile From(string json)
        {
            return JsonSerializerWrapper.Deserialize<KeyFile>(json);
        }

        /// <summary>
        /// Load a KeyFile object from a json file path
        /// </summary>
        /// <param name="filePath">JSON String</param>
        /// <returns>KeyFile object</returns>
        public static KeyFile FromFilePath(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return JsonSerializerWrapper.Deserialize<KeyFile>(json);
        }
    }

    public class Crypto
    {
        public string          Ciphertext   { get; set; }
        public CipherStructure Cipherparams { get; set; }
        public string          Cipher       { get; set; }
        public string          Kdf          { get; set; }
        public KdfSructure     Kdfparams    { get; set; }
        public string          Mac          { get; set; }

        public class CipherStructure
        {
            public string Iv { get; set; }
        }

        public class KdfSructure
        {
            public int    dklen { get; set; }
            public string Salt  { get; set; }
            public int    N     { get; set; }
            public int    r     { get; set; }
            public int    p     { get; set; }
        }
    }
}
