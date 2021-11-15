using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Erdcsharp.Configuration;
using Erdcsharp.Domain.Helper;
using Erdcsharp.Provider.Dtos;

namespace Erdcsharp.Provider
{
    public class ElrondProvider : IElrondProvider
    {
        private readonly HttpClient _httpClient;

        public ElrondProvider(HttpClient httpClient, ElrondNetworkConfiguration configuration = null)
        {
            _httpClient = httpClient;
            if (configuration != null)
            {
                _httpClient.BaseAddress = configuration.GatewayUri;
            }
        }

        public async Task<ConfigDataDto> GetNetworkConfig()
        {
            var response = await _httpClient.GetAsync("network/config");

            var content = await response.Content.ReadAsStringAsync();
            var result  = JsonSerializerWrapper.Deserialize<ElrondGatewayResponseDto<ConfigDataDto>>(content);
            result.EnsureSuccessStatusCode();
            return result.Data;
        }

        public async Task<AccountDto> GetAccount(string address)
        {
            var response = await _httpClient.GetAsync($"address/{address}");

            var content = await response.Content.ReadAsStringAsync();
            var result  = JsonSerializerWrapper.Deserialize<ElrondGatewayResponseDto<AccountDataDto>>(content);
            result.EnsureSuccessStatusCode();
            return result.Data.Account;
        }

        public async Task<EsdtTokenDataDto> GetEsdtTokens(string address)
        {
            var response = await _httpClient.GetAsync($"address/{address}/esdt");

            var content = await response.Content.ReadAsStringAsync();
            var result  = JsonSerializerWrapper.Deserialize<ElrondGatewayResponseDto<EsdtTokenDataDto>>(content);
            result.EnsureSuccessStatusCode();
            return result.Data;
        }

        public async Task<EsdtItemDto> GetEsdtNftToken(string address, string tokenIdentifier, ulong tokenId)
        {
            var response = await _httpClient.GetAsync($"address/{address}/nft/{tokenIdentifier}/nonce/{tokenId}");

            var content = await response.Content.ReadAsStringAsync();
            var result  = JsonSerializerWrapper.Deserialize<ElrondGatewayResponseDto<EsdtItemDto>>(content);
            result.EnsureSuccessStatusCode();
            return result.Data;
        }

        public async Task<EsdtTokenData> GetEsdtToken(string address, string tokenIdentifier)
        {
            var response = await _httpClient.GetAsync($"address/{address}/esdt/{tokenIdentifier}");

            var content = await response.Content.ReadAsStringAsync();
            var result  = JsonSerializerWrapper.Deserialize<ElrondGatewayResponseDto<EsdtTokenData>>(content);
            result.EnsureSuccessStatusCode();
            return result.Data;
        }

        public async Task<CreateTransactionResponseDataDto> SendTransaction(TransactionRequestDto transactionRequestDto)
        {
            var raw      = JsonSerializerWrapper.Serialize(transactionRequestDto);
            var payload  = new StringContent(raw, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("transaction/send", payload);

            var content = await response.Content.ReadAsStringAsync();
            var result =
                JsonSerializerWrapper.Deserialize<ElrondGatewayResponseDto<CreateTransactionResponseDataDto>>(content);
            result.EnsureSuccessStatusCode();
            return result.Data;
        }

        public async Task<TransactionCostDataDto> GetTransactionCost(TransactionRequestDto transactionRequestDto)
        {
            var raw      = JsonSerializerWrapper.Serialize(transactionRequestDto);
            var payload  = new StringContent(raw, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("transaction/cost", payload);

            var content = await response.Content.ReadAsStringAsync();
            var result  = JsonSerializerWrapper.Deserialize<ElrondGatewayResponseDto<TransactionCostDataDto>>(content);
            result.EnsureSuccessStatusCode();
            return result.Data;
        }

        public async Task<QueryVmResultDataDto> QueryVm(QueryVmRequestDto queryVmRequestDto)
        {
            var raw      = JsonSerializerWrapper.Serialize(queryVmRequestDto);
            var payload  = new StringContent(raw, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("vm-values/query", payload);

            var content = await response.Content.ReadAsStringAsync();
            var result  = JsonSerializerWrapper.Deserialize<ElrondGatewayResponseDto<QueryVmResultDataDto>>(content);
            result.EnsureSuccessStatusCode();
            return result.Data;
        }

        public async Task<TransactionDto> GetTransactionDetail(string txHash)
        {
            var response = await _httpClient.GetAsync($"transaction/{txHash}?withResults=true");

            var content = await response.Content.ReadAsStringAsync();
            var result  = JsonSerializerWrapper.Deserialize<ElrondGatewayResponseDto<TransactionResponseData>>(content);
            result.EnsureSuccessStatusCode();
            return result.Data.Transaction;
        }
    }
}
