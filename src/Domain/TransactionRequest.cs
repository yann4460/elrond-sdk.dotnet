using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Erdcsharp.Domain.Codec;
using Erdcsharp.Domain.Helper;
using Erdcsharp.Domain.SmartContracts;
using Erdcsharp.Domain.Values;
using Erdcsharp.Provider;
using Erdcsharp.Provider.Dtos;

namespace Erdcsharp.Domain
{
    public class TransactionRequest
    {
        private static readonly BinaryCodec BinaryCoder = new BinaryCodec();
        private readonly        string      _chainId;
        private const           int         TransactionVersion = 4;

        public Account     Account  { get; }
        public Address     Sender   { get; }
        public long        Nonce    { get; }
        public long        GasPrice { get; }
        public TokenAmount Value    { get; private set; }
        public Address     Receiver { get; private set; }
        public GasLimit    GasLimit { get; private set; }
        public string      Data     { get; private set; }

        private TransactionRequest(Account account, NetworkConfig networkConfig)
        {
            _chainId = networkConfig.ChainId;
            Account  = account;
            Sender   = account.Address;
            Receiver = Address.Zero();
            Value    = TokenAmount.Zero();
            Nonce    = account.Nonce;
            GasLimit = new GasLimit(networkConfig.MinGasLimit);
            GasPrice = networkConfig.MinGasPrice;
        }

        public static TransactionRequest Create(Account account, NetworkConfig networkConfig)
        {
            return new TransactionRequest(account, networkConfig);
        }

        public static TransactionRequest Create(Account account, NetworkConfig networkConfig, Address receiver,
                                                TokenAmount value)
        {
            return new TransactionRequest(account, networkConfig) {Receiver = receiver, Value = value};
        }

        public static TransactionRequest CreateDeploySmartContractTransactionRequest(
            NetworkConfig networkConfig,
            Account account,
            CodeArtifact codeArtifact,
            CodeMetadata codeMetadata,
            params IBinaryType[] args)
        {
            var transaction = Create(account, networkConfig);
            var data        = $"{codeArtifact.Value}@{Constants.ArwenVirtualMachine}@{codeMetadata.Value}";
            if (args.Any())
            {
                data = args.Aggregate(data,
                                      (c, arg) => c + $"@{Converter.ToHexString(BinaryCoder.EncodeTopLevel(arg))}");
            }

            transaction.SetData(data);
            transaction.SetGasLimit(GasLimit.ForSmartContractCall(networkConfig, transaction));
            return transaction;
        }

        public static TransactionRequest CreateCallSmartContractTransactionRequest(
            NetworkConfig networkConfig,
            Account account,
            Address address,
            string functionName,
            TokenAmount value,
            params IBinaryType[] args)
        {
            var transaction = Create(account, networkConfig, address, value);
            var data        = $"{functionName}";
            if (args.Any())
            {
                data = args.Aggregate(data,
                                      (c, arg) => c + $"@{Converter.ToHexString(BinaryCoder.EncodeTopLevel(arg))}");
            }

            transaction.SetData(data);
            transaction.SetGasLimit(GasLimit.ForSmartContractCall(networkConfig, transaction));
            return transaction;
        }

        public void SetGasLimit(GasLimit gasLimit)
        {
            GasLimit = gasLimit;
        }

        public void SetData(string data)
        {
            var dataBytes = Encoding.UTF8.GetBytes(data);
            Data = Convert.ToBase64String(dataBytes);
        }

        public string GetDecodedData()
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(Data));
        }

        public TransactionRequestDto GetTransactionRequest()
        {
            var transactionRequestDto = new TransactionRequestDto
            {
                ChainID   = _chainId,
                Data      = Data,
                GasLimit  = GasLimit.Value,
                Receiver  = Receiver.Bech32,
                Sender    = Sender.Bech32,
                Value     = Value.ToString(),
                Version   = TransactionVersion,
                Nonce     = Nonce,
                GasPrice  = GasPrice,
                Signature = null
            };

            return transactionRequestDto;
        }

        public async Task<Transaction> Send(IElrondProvider provider, Wallet wallet)
        {
            var transactionRequestDto = GetTransactionRequest();
            var account               = wallet.GetAccount();
            await account.Sync(provider);

            if (Value.Value > account.Balance.Value)
                throw new Exception($"Insufficient funds, required : {Value} and got {account.Balance}");

            if (Nonce != account.Nonce)
                throw new Exception($"Incorrect nonce, account nonce is {account.Nonce}, not {Nonce}");


            var json    = JsonSerializerWrapper.Serialize(transactionRequestDto);
            var message = Encoding.UTF8.GetBytes(json);

            transactionRequestDto.Signature = wallet.Sign(message);

            var result = await provider.SendTransaction(transactionRequestDto);
            Account.IncrementNonce();
            return Transaction.From(result);
        }

        public void AddArgument(IBinaryType[] args)
        {
            if (!args.Any())
                return;

            var binaryCodec = new BinaryCodec();
            var decodedData = GetDecodedData();
            var data = args.Aggregate(decodedData,
                                      (c, arg) => c + $"@{Converter.ToHexString(binaryCodec.EncodeTopLevel(arg))}");
            SetData(data);
        }
    }
}
