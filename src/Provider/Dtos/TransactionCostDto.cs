namespace Elrond.Dotnet.Sdk.Provider.Dtos
{
    public class TransactionCostDto
    {
        public TransactionCostDataDto Data { get; set; }
        public string Code { get; set; }
        public string Error { get; set; }
    }

    public class TransactionCostDataDto
    {
        public long TxGasUnits { get; set; }
        public string ReturnMessage { get; set; }
    }
}