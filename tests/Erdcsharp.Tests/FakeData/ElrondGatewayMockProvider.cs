using System;
using Erdcsharp.Provider;
using Erdcsharp.Provider.Dtos;
using Moq;

namespace Erdcsharp.UnitTests.FakeData
{
    public class ElrondGatewayMockProvider
    {
        public readonly Mock<IElrondProvider> MockProvider;

        public ElrondGatewayMockProvider()
        {
            MockProvider = new Mock<IElrondProvider>();
            MockProvider.Setup(p => p.GetAccount(It.IsAny<string>())).ReturnsAsync((string address) =>
                                                                                       new AccountDto
                                                                                       {
                                                                                           Address  = address,
                                                                                           Balance  = "99882470417129999997",
                                                                                           Nonce    = 2555546,
                                                                                           Username = "elrond"
                                                                                       });
        }

        public void SetTransactionDetailResult(string finalStatus = "success", SmartContractResultDto[] scResult = null)
        {
            var callNumber = 0;
            var transactionResponse = new TransactionDto();

            MockProvider.Setup(s => s.GetTransactionDetail(It.IsAny<string>())).ReturnsAsync(() =>
            {
                switch (callNumber)
                {
                    case 0:
                        transactionResponse.Status = "pending";
                        break;
                    case 1:
                        transactionResponse.Status               = finalStatus;
                        transactionResponse.SmartContractResults = scResult;
                        break;
                    case 2:
                        transactionResponse.HyperblockHash = Guid.NewGuid().ToString();
                        break;
                }

                callNumber++;
                return transactionResponse;
            });
        }
    }
}
