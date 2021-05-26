using System;
using Elrond.Dotnet.Sdk.Manager;
using Elrond.Dotnet.Sdk.Provider;
using Microsoft.Extensions.DependencyInjection;

namespace Elrond.Dotnet.Sdk
{
    public static class Extension
    {
        public enum Network
        {
            MainNet,
            TestNet,
            DevNet
        }

        public static IServiceCollection AddElrondProvider(this IServiceCollection services, Network network)
        {
            var configuration = new ElrondNetworkConfiguration(network);
            services.AddSingleton(configuration);
            services.AddHttpClient<IElrondProvider, ElrondProvider>(client => { client.BaseAddress = configuration.GatewayUri; });
            services.AddTransient<IEsdtTokenManager, EsdtTokenManager>();
            return services;
        }

        public class ElrondNetworkConfiguration
        {
            public Network Network { get; }

            public ElrondNetworkConfiguration(Network network)
            {
                Network = network;
                switch (network)
                {
                    case Network.MainNet:
                        GatewayUri = new Uri("https://gateway.elrond.com");
                        ApiUri = new Uri("https://api.elrond.com");
                        ExplorerUri = new Uri("https://explorer.elrond.com/");
                        break;
                    case Network.TestNet:
                        GatewayUri = new Uri("https://testnet-gateway.elrond.com");
                        ApiUri = new Uri("https://testnet-api.elrond.com");
                        ExplorerUri = new Uri("https://testnet-explorer.elrond.com/");
                        break;
                    case Network.DevNet:
                        GatewayUri = new Uri("https://devnet-gateway.elrond.com");
                        ApiUri = new Uri("https://devnet-api.elrond.com");
                        ExplorerUri = new Uri("https://devnet-explorer.elrond.com/");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(network), network, null);
                }
            }

            public Uri GatewayUri { get; }
            public Uri ApiUri { get; }
            public Uri ExplorerUri { get; }
        }
    }
}