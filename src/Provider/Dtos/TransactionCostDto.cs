namespace Elrond.Dotnet.Sdk.Provider.Dtos
{
    public class TransactionCostDto
    {
        public Data Data { get; set; }
        public string Code { get; set; }
        public string Error { get; set; }
    }

    public class Data
    {
        public long TxGasUnits { get; set; }
        public string ReturnMessage { get; set; }
    }
}