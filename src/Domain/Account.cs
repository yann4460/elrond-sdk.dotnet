using System.Threading.Tasks;
using Sdk.Provider;

namespace Sdk.Domain
{
    public class Account
    {
        public Address Address { get; }
        public Balance Balance { get; private set; }
        public int Nonce { get; private set; }
        public int TxCount { get; private set; }
        public string Code { get; private set; }
        public string UserName { get; private set; }

        public Account(Address address)
        {
            Address = address;
            Nonce = 0;
        }

        /// <summary>
        /// Synchronizes account properties (such as nonce, balance) with the ones queried from the Network
        /// </summary>
        /// <param name="provider">the Network provider</param>
        /// <returns></returns>
        public async Task Sync(IElrondProvider provider)
        {
            var accountDto = await provider.GetAccount(Address.Bech32);
            Balance = new Balance(accountDto.Balance);
            Nonce = accountDto.Nonce;
            Code = accountDto.Code;
            UserName = accountDto.UserName;
            TxCount = accountDto.TxCount;
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