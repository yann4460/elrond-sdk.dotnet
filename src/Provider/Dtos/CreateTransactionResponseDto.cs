namespace Elrond.Dotnet.Sdk.Provider.Dtos
{
    public class CreateTransactionResponseDto
    {
        public CreateTransactionResponseDataDto Data { get; set; }
        public string Error { get; set; }
        public string Code { get; set; }
    }

    public class CreateTransactionResponseDataDto
    {
        public string TxHash { get; set; }
    }
}