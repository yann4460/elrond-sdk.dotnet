using System;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Erdcsharp.Domain.Helper;
using Erdcsharp.Domain.Values;
using Erdcsharp.Provider;
using Erdcsharp.Provider.Dtos;

namespace Erdcsharp.Domain
{
    /// <summary>
    /// Elrond Standard Digital Token
    /// </summary>
    public class EsdtToken
    {
        public TokenIdentifierValue TokenIdentifier { get; private set; }
        public string               Name            { get; private set; }
        public EsdtTokenType        TokenType       { get; private set; }
        public BigInteger           Balance         { get; private set; }
        public NftTokenData         TokenData       { get; private set; }
        public EsdtTokenProperties  Properties      { get; private set; }

        public static EsdtToken From(EsdtItemDto esdt, EsdtTokenProperties properties)
        {
            switch (properties.Type)
            {
                case EsdtTokenType.FungibleESDT:
                    return new EsdtToken
                    {
                        Name            = esdt.Name,
                        TokenIdentifier = TokenIdentifierValue.From(esdt.TokenIdentifier),
                        TokenType       = properties.Type,
                        Balance         = BigInteger.Parse(esdt.Balance),
                        Properties      = properties,
                        TokenData       = null
                    };
                case EsdtTokenType.NonFungibleESDT:
                case EsdtTokenType.SemiFungibleESDT:
                    var uris = esdt.Uris
                                   .Select(u => new Uri(Encoding.UTF8.GetString(Convert.FromBase64String(u))))
                                   .ToArray();
                    return new EsdtToken
                    {
                        Name = esdt.Name,
                        TokenIdentifier = TokenIdentifierValue.From(esdt.TokenIdentifier.Substring(0,
                                                                                                   esdt.TokenIdentifier.IndexOf('-') + 7)),
                        TokenType  = properties.Type,
                        Balance    = BigInteger.Parse(esdt.Balance),
                        Properties = properties,
                        TokenData = new NftTokenData
                        {
                            TokenId   = esdt.Nonce,
                            Royalties = ushort.Parse(esdt.Royalties),
                            Creator   = Address.FromBech32(esdt.Creator),
                            Uris      = uris,
                            Hash      = esdt.Hash == null ? null : Converter.FromHexString(esdt.Hash),
                        }
                    };
                default:
                    throw new ArgumentOutOfRangeException(nameof(properties.Type));
            }
        }

        public static EsdtToken From(EsdtTokenData esdt, EsdtTokenProperties properties)
        {
            var token = esdt.TokenData;
            return new EsdtToken
            {
                Name            = token.Name,
                Properties      = properties,
                TokenIdentifier = TokenIdentifierValue.From(token.TokenIdentifier),
                Balance         = BigInteger.Parse(token.Balance),
                TokenType       = EsdtTokenType.FungibleESDT,
                TokenData       = null
            };
        }

        public class EsdtTokenProperties
        {
            public static async Task<EsdtTokenProperties> FromNetwork(IElrondProvider elrondProvider,
                                                                      string tokenIdentifier)
            {
                var response = await elrondProvider.QueryVm(new QueryVmRequestDto
                {
                    ScAddress = Constants.SmartContractAddress.EsdtSmartContract,
                    FuncName  = "getTokenProperties",
                    Args      = new[] {Encoding.UTF8.GetBytes(tokenIdentifier).ToHex()}
                });
                var properties = new EsdtTokenProperties();
                var index      = 0;

                //See : https://docs.elrond.com/developers/esdt-tokens/#get-esdt-token-properties
                foreach (var base64 in response.Data.ReturnData)
                {
                    var bytes   = Convert.FromBase64String(base64);
                    var decoded = Encoding.UTF8.GetString(bytes);
                    switch (index)
                    {
                        case 0:
                            properties.Name = decoded;
                            break;
                        case 1:
                            properties.Type = (EsdtTokenType)Enum.Parse(typeof(EsdtTokenType), decoded, true);
                            break;
                        case 2:
                            properties.Address = Address.FromBytes(bytes);
                            break;
                        case 3:
                            properties.TotalSupply = BigInteger.Parse(decoded);
                            break;
                        case 4:
                            properties.BurnValue = BigInteger.Parse(decoded);
                            break;
                        default:
                            var split         = decoded.Split('-').ToArray();
                            var propertyName  = split[0];
                            var propertyValue = split[1];

                            var property = properties.GetType().GetProperties().SingleOrDefault(p =>
                                                                                                    p.Name.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase));
                            if (property != null)
                            {
                                if (property.PropertyType.Name == "BigInteger")
                                {
                                    property.SetValue(properties, BigInteger.Parse(propertyValue), null);
                                }
                                else
                                {
                                    property.SetValue(properties,
                                                      Convert.ChangeType(propertyValue, property.PropertyType), null);
                                }
                            }

                            break;
                    }

                    index++;
                }

                return properties;
            }

            public string        Name        { get; set; }
            public EsdtTokenType Type        { get; set; }
            public Address       Address     { get; set; }
            public BigInteger    TotalSupply { get; set; }
            public BigInteger    BurnValue   { get; set; }

            /// <summary>
            /// The decimal precision
            /// </summary>
            public int NumDecimals { get; set; }

            /// <summary>
            /// Prevent all transactions of the token, apart from minting and burning
            /// </summary>
            public bool IsPaused { get; set; }

            /// <summary>
            /// the token manager may change these properties
            /// </summary>
            public bool CanUpgrade { get; set; }

            /// <summary>
            /// More units of this token can be minted by the token manager after initial issuance, increasing the supply
            /// </summary>
            public bool CanMint { get; set; }

            /// <summary>
            /// Users may "burn" some of their tokens, reducing the supply
            /// </summary>
            public bool CanBurn { get; set; }

            /// <summary>
            /// Token management can be transferred to a different account
            /// </summary>
            public bool CanChangeOwner { get; set; }

            /// <summary>
            /// The token manager may prevent all transactions of the token, apart from minting and burning
            /// </summary>
            public bool CanPause { get; set; }

            /// <summary>
            /// The token manager may freeze the token balance in a specific account, preventing transfers to and from that account
            /// </summary>
            public bool CanFreeze { get; set; }

            /// <summary>
            /// The token manager may wipe out the tokens held by a frozen account, reducing the supply
            /// </summary>
            public bool CanWipe { get; set; }

            /// <summary>
            /// the token manager can assign a specific role(s)
            /// </summary>
            public bool CanAddSpecialRoles { get; set; }

            public bool CanTransferNFTCreateRole { get; set; }

            public bool NFTCreateStopped { get; set; }

            public BigInteger NumWiped { get; set; }
        }

        public class NftTokenData
        {
            public BigInteger TokenId   { get; set; }
            public byte[]     Hash      { get; set; }
            public int        Royalties { get; set; }
            public Address    Creator   { get; set; }
            public Uri[]      Uris      { get; set; }
        }
    }
}
