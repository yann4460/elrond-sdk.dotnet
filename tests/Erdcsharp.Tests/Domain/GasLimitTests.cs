using System.Threading.Tasks;
using Erdcsharp.Domain;
using Erdcsharp.Provider;
using Erdcsharp.Provider.Dtos;
using Moq;
using NUnit.Framework;

namespace Erdcsharp.UnitTests.Domain
{
    [TestFixture(Category = "UnitTests")]
    public class GasLimitTests
    {
        private IElrondProvider _elrondProvider;

        [SetUp]
        public void Setup()
        {
            var mock = new Mock<IElrondProvider>();
            mock.Setup(s => s.GetNetworkConfig()).ReturnsAsync(new ConfigDataDto
            {
                Config = new ConfigDto
                {
                    erd_min_gas_limit     = 50000,
                    erd_gas_per_data_byte = 1500,
                    erd_min_gas_price     = 1000000000
                }
            });
            _elrondProvider = mock.Object;
        }

        [Test]
        public async Task GasLimit_Should_Compute_Gas_ForTransfer()
        {
            // Arrange
            var constants          = await NetworkConfig.GetFromNetwork(_elrondProvider);
            var address            = Address.FromBech32("erd1qqqqqqqqqqqqqpgq3wltgm6g8n6telq3wz2apgjqcydladdtu4cq3ch0l0");
            var transactionRequest = TransactionRequest.Create(new Account(address), constants);

            transactionRequest.SetData("KLJHGFhjbnklmjghfdhfkjl");

            // Act
            var gasLimit = GasLimit.ForTransfer(constants, transactionRequest);

            // Assert
            Assert.AreEqual(84500, gasLimit.Value);
        }
    }
}
