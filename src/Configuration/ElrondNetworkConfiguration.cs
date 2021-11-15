using System;

namespace Erdcsharp.Configuration
{
    public class ElrondNetworkConfiguration
    {
        public ElrondNetworkConfiguration(Network network)
        {
            Network = network;
            switch (network)
            {
                case Network.MainNet:
                    GatewayUri  = new Uri("https://gateway.elrond.com");
                    ExplorerUri = new Uri("https://explorer.elrond.com/");
                    break;
                case Network.TestNet:
                    GatewayUri  = new Uri("https://testnet-gateway.elrond.com");
                    ExplorerUri = new Uri("https://testnet-explorer.elrond.com/");
                    break;
                case Network.DevNet:
                    GatewayUri  = new Uri("https://devnet-gateway.elrond.com");
                    ExplorerUri = new Uri("https://devnet-explorer.elrond.com/");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(network), network, null);
            }
        }

        public Network Network     { get; }
        public Uri     GatewayUri  { get; }
        public Uri     ExplorerUri { get; }
    }
}
