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
        public string Hash { get; set; }

        public Dictionary<string, string> Attributes { get; set; }

        public int Royalties { get; set; }

        public AddressValue Creator { get; set; }
        public Uri[] Uris { get; set; }

        public static EsdtToken From(EsdtItemDto esdt)
        {
            //var attributes = esdt.Attributes.Split(';', StringSplitOptions.RemoveEmptyEntries);
            //var attributesDic = attributes.ToDictionary(
            //    s => s.Split(":", StringSplitOptions.RemoveEmptyEntries).First(),
            //    v => v.Split(":", StringSplitOptions.RemoveEmptyEntries).Last());

            var uris = esdt.Uris.Select(u => new Uri(Encoding.UTF8.GetString(Convert.FromBase64String(u)))).ToArray();
            return new EsdtToken
            {
                Name = esdt.Name,
                TokenIdentifier = TokenIdentifierValue.From(esdt.TokenIdentifier),
                TokenId = esdt.Nonce,
                Royalties = ushort.Parse(esdt.Royalties),
                //Attributes = attributesDic,
                Creator = AddressValue.FromBech32(esdt.Creator),
                Uris = uris,
                Hash = esdt.Hash
            };
        }
    }
}