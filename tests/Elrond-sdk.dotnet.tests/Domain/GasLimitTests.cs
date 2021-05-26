using System.Threading.Tasks;
using Elrond.Dotnet.Sdk.Domain;
using Elrond.Dotnet.Sdk.Domain.Values;
using Elrond.Dotnet.Sdk.Provider;
using Elrond.Dotnet.Sdk.Provider.Dtos;
using Moq;
using NUnit.Framework;

namespace Elrond_sdk.dotnet.tests.Domain
{
    public class GasLimitTests
    {
        private IElrondProvider _elrondProvider;

        [SetUp]
        public void Setup()
        {
            var mock = new Mock<IElrondProvider>();
            mock.Setup(s => s.GetConstants()).ReturnsAsync(new ConfigDataDto
            {
                Config = new ConfigDto
                {
                    erd_min_gas_limit = 50000,
                    erd_gas_per_data_byte = 1500,
                    erd_min_gas_price = 1000000000
                }
            });
            _elrondProvider = mock.Object;
        }

        [Test]
        public async Task GasLimit_Should_Compute_Gas_ForTransfer()
        {
            // Arrange
            var constants = await Constants.GetFromNetwork(_elrondProvider);
            var address = AddressValue.FromBech32("erd1qqqqqqqqqqqqqpgq3wltgm6g8n6telq3wz2apgjqcydladdtu4cq3ch0l0");
            var transactionRequest = TransactionRequest.CreateTransaction(new Account(address), constants);

            transactionRequest.SetData("KLJHGFhjbnklmjghfdhfkjl");

            // Act
            var gasLimit = GasLimit.ForTransfer(constants, transactionRequest);

            // Assert
            Assert.AreEqual(84500, gasLimit.Value);
        }
    }
}