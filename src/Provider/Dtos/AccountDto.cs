namespace Elrond.Dotnet.Sdk.Provider.Dtos
{
    public class AccountDto
    {
        public string Address { get; set; }
        public int Nonce { get; set; }
        public string Balance { get; set; }
        public string Code { get; set; }
        public string UserName { get; set; }
        public int TxCount { get; set; }
    }
}
