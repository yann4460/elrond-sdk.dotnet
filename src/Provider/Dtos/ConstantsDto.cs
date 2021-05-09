namespace Elrond.Dotnet.Sdk.Provider.Dtos
{
    public class ConstantsDto
    {
        public string ChainId { get; set; }
        public long GasPerDataByte { get; set; }
        public long MinGasLimit { get; set; }
        public long MinGasPrice { get; set; }
        public int MinTransactionVersion { get; set; }
    }
}