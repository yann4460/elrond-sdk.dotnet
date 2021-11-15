using System.Net.Http;
using System.Threading.Tasks;
using Erdcsharp.Configuration;
using Erdcsharp.Domain;
using Erdcsharp.Domain.Abi;
using Erdcsharp.Domain.SmartContracts;
using Erdcsharp.Domain.Values;
using Erdcsharp.Provider;
using Erdcsharp.UnitTests.FakeData;
using NUnit.Framework;

namespace Erdcsharp.IntegrationTests
{
    [TestFixture(Category = "LongRunning", Description = "Smart contracts interaction usage")]
    public class SmartContractTests
    {
        private IElrondProvider   _provider;
        private TestWalletFactory _testWalletFactory;

        [SetUp]
        public void Setup()
        {
            _provider          = new ElrondProvider(new HttpClient(), new ElrondNetworkConfiguration(Network.TestNet));
            _testWalletFactory = new TestWalletFactory();
        }

        [Test(Description = "Deploy adder smart contract from alice account with an initial value")]
        public async Task Should_Deploy_Adder_SmartContract_And_Set_Initial_Value()
        {
            var networkConfig = await NetworkConfig.GetFromNetwork(_provider);
            var alice         = _testWalletFactory.Alice;
            var aliceAccount  = alice.GetAccount();
            await aliceAccount.Sync(_provider);

            var code         = CodeArtifact.FromFilePath("FakeData/SmartContracts/adder/adder.wasm");
            var codeMetadata = new CodeMetadata(false, true, false);
            var initialValue = NumericValue.BigUintValue(10);

            var deployTxRequest = TransactionRequest.CreateDeploySmartContractTransactionRequest(
                                                                                                 networkConfig,
                                                                                                 aliceAccount,
                                                                                                 code,
                                                                                                 codeMetadata,
                                                                                                 initialValue);

            var deployTx = await deployTxRequest.Send(_provider, alice);
            await deployTx.AwaitExecuted(_provider);
            await deployTx.AwaitNotarized(_provider);

            var scAddress = SmartContract.ComputeAddress(deployTxRequest);
            Assert.That(scAddress, Is.Not.Null);

            await CallSmartContract(networkConfig, scAddress);
        }

        private async Task CallSmartContract(NetworkConfig networkConfig, Address smartContractAddress)
        {
            var wallet  = _testWalletFactory.Bob;
            var account = wallet.GetAccount();
            await account.Sync(_provider);
            var txRequest = TransactionRequest.CreateCallSmartContractTransactionRequest(
                                                                                         networkConfig,
                                                                                         account,
                                                                                         smartContractAddress,
                                                                                         "add",
                                                                                         TokenAmount.Zero(),
                                                                                         NumericValue.BigUintValue(10));

            var tx = await txRequest.Send(_provider, wallet);
            await tx.AwaitExecuted(_provider);
            await tx.AwaitNotarized(_provider);

            await QuerySmartContract(smartContractAddress);
            await QuerySmartContractWithAbi(smartContractAddress);
        }

        private async Task QuerySmartContract(Address smartContractAddress)
        {
            var outputType = TypeValue.BigUintTypeValue;
            var queryResult = await SmartContract.QuerySmartContract<NumericValue>(
                                                                                   _provider,
                                                                                   smartContractAddress,
                                                                                   outputType,
                                                                                   "getSum");

            Assert.That(queryResult.Number.ToString(), Is.EqualTo("20"));
        }

        private async Task QuerySmartContractWithAbi(Address smartContractAddress)
        {
            var abiDefinition = AbiDefinition.FromFilePath("FakeData/SmartContracts/adder/adder.abi.json");
            var queryResult = await SmartContract.QuerySmartContractWithAbiDefinition<NumericValue>(
                                                                                                    _provider,
                                                                                                    smartContractAddress,
                                                                                                    abiDefinition,
                                                                                                    "getSum");

            Assert.That(queryResult.Number.ToString(), Is.EqualTo("20"));
        }
    }
}
