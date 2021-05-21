using System.Threading.Tasks;
using Elrond.Dotnet.Sdk.Domain.Values;
using Elrond.Dotnet.Sdk.Provider;

namespace Elrond.Dotnet.Sdk.Domain
{
    public class Account
    {
        public AddressValue Address { get; }
        public Balance Balance { get; private set; }
        public int Nonce { get; private set; }
        public string Code { get; private set; }
        public string UserName { get; private set; }

        public Account(AddressValue address)
        {
            Address = address;
            Nonce = 0;
        }

        /// <summary>
        /// Synchronizes account properties (such as nonce, balance) with the ones queried from the Network
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public async Task Sync(IElrondProvider provider)
        {
            var accountDto = await provider.GetAccount(Address.Bech32);
            Balance = new Balance(accountDto.Data.Account.Balance);
            Nonce = accountDto.Data.Account.Nonce;
            Code = accountDto.Code;
            UserName = accountDto.Data.Account.Username;
        }

        /// <summary>
        /// Increments (locally) the nonce (the account sequence number).
        /// </summary>
        public void IncrementNonce()
        {
            Nonce++;
        }
    }
}