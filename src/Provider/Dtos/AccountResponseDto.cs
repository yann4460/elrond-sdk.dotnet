namespace Erdcsharp.Provider.Dtos
{
    public class AccountDataDto
    {
        public AccountDto Account { get; set; }
    }

    public class AccountDto
    {
        public string Address  { get; set; }
        public long   Nonce    { get; set; }
        public string Balance  { get; set; }
        public string Username { get; set; }
    }
}
