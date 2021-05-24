using System.Collections.Generic;

namespace Elrond.Dotnet.Sdk.Provider.Dtos
{
    public class ESDTTokenDataDto
    {
        public Dictionary<string, EsdtItemDto> Esdts { get; set; }
    }

    public class EsdtDataDto
    {
        public string Balance { get; set; }
        public string Properties { get; set; }
        public string TokenIdentifier { get; set; }
    }

    public class EsdtTokenData
    {
        public EsdtItemDto TokenData { get; set; }
    }

    public class EsdtItemDto
    {
        //public string Attributes { get; set; }
        public string Balance { get; set; }
        public string Creator { get; set; }
        public string Hash { get; set; }
        public string Name { get; set; }
        public ulong Nonce { get; set; }
        public string Royalties { get; set; }
        public string TokenIdentifier { get; set; }
        public string[] Uris { get; set; }
    }
}