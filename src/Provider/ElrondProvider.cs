using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Elrond.Dotnet.Sdk.Provider.Dtos;

namespace Elrond.Dotnet.Sdk.Provider
{
    public class ElrondProvider : IElrondProvider
    {
        private readonly HttpClient _httpClient;

        private static readonly JsonSerializerOptions SerializeOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public ElrondProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ConfigDataDto> GetConstants()
        {
            var response = await _httpClient.GetAsync("network/config");
            var result = await response.Content.ReadFromJsonAsync<ElrondGatewayResponseDto<ConfigDataDto>>();
            result.EnsureSuccessStatusCode();

            return result.Data;
        }

        public async Task<AccountDataDto> GetAccount(string address)
        {
            var response = await _httpClient.GetAsync($"address/{address}");

            var result = await response.Content.ReadFromJsonAsync<ElrondGatewayResponseDto<AccountDataDto>>();
            result.EnsureSuccessStatusCode();

            return result.Data;
        }

        public async Task<ESDTTokenDataDto> GetEsdtTokens(string address)
        {
            var response = await _httpClient.GetAsync($"address/{address}/esdt");
            var result = await response.Content.ReadFromJsonAsync<ElrondGatewayResponseDto<ESDTTokenDataDto>>();
            result.EnsureSuccessStatusCode();

            return result.Data;
        }

        public async Task<EsdtItemDto> GetEsdtNftToken(string address, string tokenIdentifier, ulong tokenId)
        {
            var response = await _httpClient.GetAsync($"address/{address}/nft/{tokenIdentifier}/nonce/{tokenId}");
            var result = await response.Content.ReadFromJsonAsync<ElrondGatewayResponseDto<EsdtItemDto>>();
            result.EnsureSuccessStatusCode();

            return result.Data;
        }

        public async Task<EsdtDataDto> GetEsdtToken(string address, string tokenIdentifier)
        {
            var response = await _httpClient.GetAsync($"address/{address}/esdt/{tokenIdentifier}");
            var result = await response.Content.ReadFromJsonAsync<ElrondGatewayResponseDto<EsdtDataDto>>();
            result.EnsureSuccessStatusCode();

            return result.Data;
        }

        public async Task<CreateTransactionResponseDataDto> SendTransaction(TransactionRequestDto transactionRequestDto)
        {
            var raw = JsonSerializer.Serialize(transactionRequestDto, SerializeOptions);
            var content = new StringContent(raw, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("transaction/send", content);

            var result = await response.Content
                .ReadFromJsonAsync<ElrondGatewayResponseDto<CreateTransactionResponseDataDto>>();
            result.EnsureSuccessStatusCode();

            return result.Data;
        }

        public async Task<TransactionCostDataDto> GetTransactionCost(TransactionRequestDto transactionRequestDto)
        {
            var raw = JsonSerializer.Serialize(transactionRequestDto, SerializeOptions);
            var content = new StringContent(raw, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("transaction/cost", content);
            var result = await response.Content.ReadFromJsonAsync<ElrondGatewayResponseDto<TransactionCostDataDto>>();
            result.EnsureSuccessStatusCode();

            return result.Data;
        }

        public async Task<QueryVmResultDataDto> QueryVm(QueryVmRequestDto queryVmRequestDto)
        {
            var raw = JsonSerializer.Serialize(queryVmRequestDto, SerializeOptions);
            var content = new StringContent(raw, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("vm-values/query", content);
            var result = await response.Content.ReadFromJsonAsync<ElrondGatewayResponseDto<QueryVmResultDataDto>>();
            result.EnsureSuccessStatusCode();

            return result.Data;
        }

        public async Task SimulateTransaction(TransactionRequestDto transactionRequestDto)
        {
            var raw = JsonSerializer.Serialize(transactionRequestDto, SerializeOptions);
            var content = new StringContent(raw, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("transaction/simulate", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task<TransactionResponseData> GetTransactionDetail(string txHash)
        {
            var response = await _httpClient.GetAsync($"transaction/{txHash}?withResults=true");
            var result = await response.Content.ReadFromJsonAsync<ElrondGatewayResponseDto<TransactionResponseData>>();
            result.EnsureSuccessStatusCode();

            return result.Data;
        }
    }
}