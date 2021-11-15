using System.Threading.Tasks;
using Erdcsharp.Provider;

namespace Erdcsharp.Domain
{
    public class Account
    {
        public Address     Address  { get; }
        public TokenAmount Balance  { get; private set; }
        public long        Nonce    { get; private set; }
        public string      UserName { get; private set; }

        public Account(Address address)
        {
            Address  = address;
            Nonce    = 0;
            Balance  = TokenAmount.Zero();
            UserName = null;
        }

        /// <summary>
        /// Synchronizes account properties (such as nonce, balance) with the ones queried from the Network
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public async Task Sync(IElrondProvider provider)
        {
            var accountDto = await provider.GetAccount(Address.Bech32);

            Balance  = TokenAmount.From(accountDto.Balance, Token.EGLD());
            Nonce    = accountDto.Nonce;
            UserName = accountDto.Username;
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
