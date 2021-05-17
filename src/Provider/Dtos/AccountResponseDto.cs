namespace Elrond.Dotnet.Sdk.Provider.Dtos
{
    public class AccountResponseDto
    {
        public AccountDataDto Data { get; set; }
        public string Error { get; set; }
        public string Code { get; set; }
    }

    public class AccountDataDto
    {
        public AccountDto Account { get; set; }
    }

    public class AccountDto
    {
        public string Address { get; set; }
        public int Nonce { get; set; }
        public string Balance { get; set; }
        public string Username { get; set; }
        public string Code { get; set; }
        public object CodeHash { get; set; }
        public string RootHash { get; set; }
    }
}