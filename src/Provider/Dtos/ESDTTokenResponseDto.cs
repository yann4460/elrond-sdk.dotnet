using System;
using System.Collections.Generic;

namespace Elrond.Dotnet.Sdk.Provider.Dtos
{
    public class ESDTTokenResponseDto
    {
        public ESDTTokenDataDto Data { get; set; }
        public string Error { get; set; }
        public string Code { get; set; }
    }

    public class ESDTTokenDataDto
    {
        public Dictionary<string, EsdtsItemDto> Esdts { get; set; }
    }

    public class EsdtsItemDto
    {
        public string Attributes { get; set; }
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