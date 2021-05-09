using System.Threading.Tasks;
using Sdk.Provider;

namespace Sdk.Domain
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

        public static Constants New()
        {
            return new Constants();
        }


        public static async Task<Constants> GetFromNetwork(IElrondProvider provider)
        {
            var constants = await provider.GetConstants();
            return new Constants
            {
                ChainId = constants.ChainId,
                GasPerDataByte = constants.GasPerDataByte,
                MinGasLimit = constants.MinGasLimit,
                MinGasPrice = constants.MinGasPrice,
                MinTransactionVersion = constants.MinTransactionVersion
            };
        }
    }
}