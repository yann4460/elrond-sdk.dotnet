using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elrond.Dotnet.Sdk.Domain;
using Elrond.Dotnet.Sdk.Domain.Values;

namespace Elrond.Dotnet.Sdk.Manager
{
    public interface IEsdtTokenManager
    {
        Task<string> IssueNonFungibleToken(Wallet wallet, string tokenName, string tokenTicker);

        Task<List<string>> GetSpecialRole(string tokenIdentifier);

        Task SetSpecialRole(Wallet wallet, string tokenIdentifier, params string[] roles);

        Task<EsdtToken> CreateNftToken(
            Wallet wallet,
            string tokenIdentifier,
            string tokenName,
            ushort royalties,
            Dictionary<string, string> attributes,
            Uri[] uris,
            string hash = null);


        Task<EsdtToken> GetNftToken(AddressValue address, string tokenIdentifier, ulong tokenId);

        Task TransferNftToken(Wallet wallet, EsdtToken token, AddressValue receiver);
    }
}