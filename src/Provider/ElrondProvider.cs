using System;
using System.Collections.Generic;
using System.Net;
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

        public async Task<ConfigResponseDto> GetConstants()
        {
            var response = await _httpClient.GetAsync("network/config");
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var badRequestResponse = await response.Content.ReadFromJsonAsync<BadRequestDto>();
                throw new Exception(badRequestResponse.Error);
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ConfigResponseDto>();
        }

        public async Task<AccountResponseDto> GetAccount(string address)
        {
            var response = await _httpClient.GetAsync($"address/{address}");
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var badRequestResponse = await response.Content.ReadFromJsonAsync<BadRequestDto>();
                throw new Exception(badRequestResponse.Error);
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<AccountResponseDto>();
        }

        public async Task<ESDTTokenResponseDto> GetESDTTokens(string address)
        {
            var response = await _httpClient.GetAsync($"address/{address}/esdt");
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var badRequestResponse = await response.Content.ReadFromJsonAsync<BadRequestDto>();
                throw new Exception(badRequestResponse.Error);
            }

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ESDTTokenResponseDto>();
            return result;
        }

        public async Task<CreateTransactionResponseDto> SendTransaction(TransactionRequestDto transactionRequestDto)
        {
            var raw = JsonSerializer.Serialize(transactionRequestDto, SerializeOptions);
            var content = new StringContent(raw, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("transaction/send", content);
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var badRequestResponse = await response.Content.ReadFromJsonAsync<BadRequestDto>();
                throw new Exception(badRequestResponse.Error);
            }

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<CreateTransactionResponseDto>();

            return result;
        }

        public async Task<TransactionCostDto> GetTransactionCost(TransactionRequestDto transactionRequestDto)
        {
            var raw = JsonSerializer.Serialize(transactionRequestDto, SerializeOptions);
            var content = new StringContent(raw, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("transaction/cost", content);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<TransactionCostDto>();
            return result;
        }

        public async Task<QueryVmResultDto> QueryVm(QueryVmRequestDto queryVmRequestDto)
        {
            var raw = JsonSerializer.Serialize(queryVmRequestDto, SerializeOptions);
            var content = new StringContent(raw, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("vm-values/query", content);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<QueryVmResultDto>();
            return result;
        }

        public async Task SimulateTransaction(TransactionRequestDto transactionRequestDto)
        {
            var raw = JsonSerializer.Serialize(transactionRequestDto, SerializeOptions);
            var content = new StringContent(raw, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("transaction/simulate", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task<TransactionResponseDto> DeployContract(TransactionRequestDto transactionRequestDto)
        {
            var raw = JsonSerializer.Serialize(transactionRequestDto, SerializeOptions);
            var content = new StringContent(raw, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("transactions", content);
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var badRequestResponse = await response.Content.ReadFromJsonAsync<BadRequestDto>();
                throw new Exception(badRequestResponse.Error);
            }

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<TransactionResponseDto>();

            return result;
        }

        public async Task<TransactionResponseDto> GetTransactionDetail(string txHash)
        {
            var response = await _httpClient.GetAsync($"transaction/{txHash}?withResults=true");
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var badRequestResponse = await response.Content.ReadFromJsonAsync<BadRequestDto>();
                throw new Exception(badRequestResponse.Error);
            }

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<TransactionResponseDto>();

            return result;
        }
    }
}