using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Elrond.Dotnet.Sdk.Provider;
using Elrond.Dotnet.Sdk.Provider.Dtos;

namespace Elrond.Dotnet.Sdk.Domain
{
    public class TransactionRequest
    {
        private readonly Account _account;
        private readonly string _chainId;
        private const int TransactionVersion = 4;

        public Address Sender { get; }
        public long Nonce { get; }
        public long GasPrice { get; }
        public Balance Value { get; private set; }
        public Address Receiver { get; private set; }
        public GasLimit GasLimit { get; private set; }
        public string Data { get; private set; }

        private TransactionRequest(Account account, Constants constants)
        {
            _account = account;
            Sender = account.Address;
            Receiver = Address.Zero();
            Value = new Balance(0);
            Nonce = account.Nonce;
            GasLimit = new GasLimit(constants.MinGasLimit);
            GasPrice = constants.MinGasPrice;
            _chainId = constants.ChainId;
        }

        public static TransactionRequest CreateTransaction(Account account, Constants constants)
        {
            return new TransactionRequest(account, constants);
        }

        public static TransactionRequest CreateTransaction(Account account, Constants constants, Address receiver,
            Balance value)
        {
            return new TransactionRequest(account, constants)
            {
                Receiver = receiver,
                Value = value
            };
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

        public TransactionRequestDto GetTransactionRequest()
        {
            var transactionRequestDto = new TransactionRequestDto
            {
                ChainID = _chainId,
                Data = Data,
                GasLimit = GasLimit.Value,
                Receiver = Receiver.Bech32,
                Sender = Sender.Bech32,
                Value = Value.ToString(),
                Version = TransactionVersion,
                Signature = null,
                Nonce = Nonce,
                GasPrice = GasPrice
            };

            return transactionRequestDto;
        }

        public async Task<Transaction> Send(Wallet wallet, IElrondProvider provider)
        {
            var transactionRequestDto = GetTransactionRequest();

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            var json = JsonSerializer.Serialize(transactionRequestDto, serializeOptions);
            var message = Encoding.UTF8.GetBytes(json);

            transactionRequestDto.Signature = wallet.Sign(message);

            var result = await provider.SendTransaction(transactionRequestDto);
            _account.IncrementNonce();
            return Transaction.From(result);
        }

        public async Task ComputeGasLimit(IElrondProvider provider)
        {
            var gasLimit = await GasLimit.ForTransaction(this, provider);
            SetGasLimit(gasLimit);
        }
    }
}