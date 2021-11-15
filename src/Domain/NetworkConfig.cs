using System.Threading.Tasks;
using Erdcsharp.Provider;

namespace Erdcsharp.Domain
{
    public class NetworkConfig
    {
        private NetworkConfig()
        {
        }

        public string ChainId               { get; set; }
        public long   GasPerDataByte        { get; set; }
        public long   MinGasLimit           { get; set; }
        public long   MinGasPrice           { get; set; }
        public int    MinTransactionVersion { get; set; }

        /// <summary>
        /// Synchronize the configuration with the network
        /// </summary>
        /// <param name="provider">Elrond provider</param>
        /// <returns>NetworkConfig</returns>
        public static async Task<NetworkConfig> GetFromNetwork(IElrondProvider provider)
        {
            var constants = await provider.GetNetworkConfig();
            return new NetworkConfig
            {
                ChainId               = constants.Config.erd_chain_id,
                GasPerDataByte        = constants.Config.erd_gas_per_data_byte,
                MinGasLimit           = constants.Config.erd_min_gas_limit,
                MinGasPrice           = constants.Config.erd_min_gas_price,
                MinTransactionVersion = constants.Config.erd_min_transaction_version
            };
        }

        /// <summary>
        /// New empty NetworkConfig
        /// </summary>
        /// <returns>NetworkConfig</returns>
        public static NetworkConfig New()
        {
            return new NetworkConfig();
        }
    }
}
