﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Elrond.Dotnet.Sdk.Provider.Dtos;

namespace Elrond.Dotnet.Sdk.Provider
{
    public interface IElrondProvider
    {
        Task<ConstantsDto> GetConstants();

        Task<AccountDto> GetAccount(string address);

        Task<IReadOnlyCollection<ESDTTokenDto>> GetESDTTokens(string address);

        Task<TransactionResponseDto> SendTransaction(TransactionRequestDto transactionRequestDto);

        Task SimulateTransaction(TransactionRequestDto transactionRequestDto);

        Task<TransactionResponseDto> GetTransactionDetail(string txHash);

        Task<TransactionCostDto> GetTransactionCost(TransactionRequestDto transactionRequestDto);
    }
}