using System.Collections.Generic;

namespace Erdcsharp.Provider.Dtos
{
    public class EsdtTokenDataDto
    {
        public Dictionary<string, EsdtItemDto> Esdts { get; set; }
    }

    public class EsdtTokenData
    {
        public EsdtItemDto TokenData { get; set; }
    }

    public class EsdtItemDto
    {
        public string   Balance         { get; set; }
        public string   Creator         { get; set; }
        public string   Name            { get; set; }
        public ulong    Nonce           { get; set; }
        public string   Hash            { get; set; }
        public string   Royalties       { get; set; }
        public string   TokenIdentifier { get; set; }
        public string[] Uris            { get; set; }
    }
}
