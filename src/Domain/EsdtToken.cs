using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Elrond.Dotnet.Sdk.Domain.Values;
using Elrond.Dotnet.Sdk.Provider.Dtos;

namespace Elrond.Dotnet.Sdk.Domain
{
    /// <summary>
    /// Elrond Standard Digital Token (Including NFT / SFT)
    /// </summary>
    public class EsdtToken
    {
        public TokenIdentifierValue TokenIdentifier { get; set; }

        public string Name { get; set; }
        public ulong TokenId { get; set; }
        public byte[] Hash { get; set; }

        public Dictionary<string, string> Attributes { get; set; }

        public int Royalties { get; set; }

        public AddressValue Creator { get; set; }
        public Uri[] Uris { get; set; }

        public EsdtTokenType TokenType { get; set; }

        public enum EsdtTokenType
        {
            ESDT,
            SFT,
            NFT
        }

        public static EsdtToken From(EsdtNftItemDto esdtNft)
        {
            //var attributes = esdtNft.Attributes.Split(';', StringSplitOptions.RemoveEmptyEntries);
            //var attributesDic = attributes.ToDictionary(
            //    s => s.Split(":", StringSplitOptions.RemoveEmptyEntries).First(),
            //    v => v.Split(":", StringSplitOptions.RemoveEmptyEntries).Last());

            var uris = esdtNft.Uris.Select(u => new Uri(Encoding.UTF8.GetString(Convert.FromBase64String(u)))).ToArray();
            return new EsdtToken
            {
                Name = esdtNft.Name,
                TokenIdentifier = TokenIdentifierValue.From(esdtNft.TokenIdentifier),
                TokenId = esdtNft.Nonce,
                Royalties = ushort.Parse(esdtNft.Royalties),
                //Attributes = attributesDic,
                Creator = AddressValue.FromBech32(esdtNft.Creator),
                Uris = uris,
                Hash = Convert.FromHexString(esdtNft.Hash)
            };
        }
    }
}