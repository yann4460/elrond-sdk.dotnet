using System.Collections.Generic;

namespace Elrond.Dotnet.Sdk.Provider.Dtos
{
    public class ESDTTokenDto
    {
        public ESDTTokenDataDto Data { get; set; }
        public string Error { get; set; }
        public string Code { get; set; }
    }

    public class ESDTTokenDataDto
    {
        public Dictionary<string, EsdtsItem> Esdts { get; set; }
    }

    public class EsdtsItem
    {
        public string Attributes { get; set; }
        public string Balance { get; set; }
        public string Creator { get; set; }
        public string Hash { get; set; }
        public string Name { get; set; }
        public int Nonce { get; set; }
        public string Royalties { get; set; }
        public string TokenIdentifier { get; set; }
        public string[] Uris { get; set; }
    }
}