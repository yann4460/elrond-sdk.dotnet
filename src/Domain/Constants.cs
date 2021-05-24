﻿using System.Threading.Tasks;
using Elrond.Dotnet.Sdk.Provider;

namespace Elrond.Dotnet.Sdk.Domain
{
    public class Constants
    {
        private Constants()
        {
        }

        public string ChainId { get; set; }
        public long GasPerDataByte { get; set; }
        public long MinGasLimit { get; set; }
        public long MinGasPrice { get; set; }
        public int MinTransactionVersion { get; set; }

        public static async Task<Constants> GetFromNetwork(IElrondProvider provider)
        {
            var constants = await provider.GetConstants();
            return new Constants
            {
                ChainId = constants.Config.erd_chain_id,
                GasPerDataByte = constants.Config.erd_gas_per_data_byte,
                MinGasLimit = constants.Config.erd_min_gas_limit,
                MinGasPrice = constants.Config.erd_min_gas_price,
                MinTransactionVersion = constants.Config.erd_min_transaction_version
            };
        }

        public static Constants New()
        {
            return new Constants();
        }
    }
}