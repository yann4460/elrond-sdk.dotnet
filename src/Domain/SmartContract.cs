using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elrond.Dotnet.Sdk.Cryptography;
using Elrond.Dotnet.Sdk.Domain.Codec;
using Elrond.Dotnet.Sdk.Domain.Values;
using Elrond.Dotnet.Sdk.Provider;
using Elrond.Dotnet.Sdk.Provider.Dtos;
using Org.BouncyCastle.Crypto.Digests;

namespace Elrond.Dotnet.Sdk.Domain
{
    public class SmartContract
    {
        private const string ArwenVirtualMachine = "0500";
        private static readonly BinaryCodec BinaryCoder = new BinaryCodec();

        public static TransactionRequest CreateDeploySmartContractTransactionRequest(
            Constants constants,
            Account account,
            Code code,
            CodeMetadata codeMetadata,
            params IBinaryType[] args)
        {
            var transaction = TransactionRequest.CreateTransaction(account, constants);
            var data = $"{code.Value}@{ArwenVirtualMachine}@{codeMetadata.Value}";
            if (args.Any())
            {
                data = args.Aggregate(data, (c, arg) => c + $"@{Convert.ToHexString(BinaryCoder.EncodeTopLevel(arg))}");
            }

            transaction.SetData(data);
            transaction.SetGasLimit(GasLimit.ForSmartContractCall(constants, transaction));

            return transaction;
        }

        public static TransactionRequest CreateUpdateSmartContractTransactionRequest(
            Constants constants,
            Account account,
            AddressValue address)
        {
            throw new NotImplementedException();
        }

        public static TransactionRequest CreateCallSmartContractTransactionRequest(
            Constants constants,
            Account account,
            AddressValue address,
            string functionName,
            Balance value,
            params IBinaryType[] args)
        {
            var transaction = TransactionRequest.CreateTransaction(account, constants, address, value);
            var data = $"{functionName}";
            if (args.Any())
            {
                data = args.Aggregate(data, (c, arg) => c + $"@{Convert.ToHexString(BinaryCoder.EncodeTopLevel(arg))}");
            }

            transaction.SetData(data);
            transaction.SetGasLimit(GasLimit.ForSmartContractCall(constants, transaction));
            return transaction;
        }

        /// <summary>
        /// Computes the address of a Smart Contract.
        /// The address is computed deterministically, from the address of the owner and the nonce of the deployment transaction.
        /// </summary>
        /// <param name="ownerAddress">The owner of the Smart Contract</param>
        /// <param name="nonce">The owner nonce used for the deployment transaction</param>
        /// <returns>The smart contract address</returns>
        public static AddressValue ComputeAddress(AddressValue ownerAddress, long nonce)
        {
            var ownerPubKey = Convert.FromHexString(ownerAddress.Hex);
            var initialPadding = new byte[8];
            var shardSelector = ownerPubKey.Skip(30).Take(2).ToArray();

            var bigNonceBuffer = BitConverter.GetBytes(nonce);

            var bytesToHash = ConcatByteArrays(ownerPubKey, bigNonceBuffer);
            var hash = CalculateHash(bytesToHash);

            var hashBytesToTake = hash.Skip(10).Take(20).ToArray();
            var vmTypeBytes = Convert.FromHexString(ArwenVirtualMachine);
            var addressBytes = ConcatByteArrays(
                initialPadding,
                vmTypeBytes,
                hashBytesToTake,
                shardSelector);

            var erdAddress = Bech32Engine.Encode("erd", addressBytes);
            return AddressValue.FromBech32(erdAddress);
        }

        /// <summary>
        /// Allows one to execute - with no side-effects - a pure function of a Smart Contract and retrieve the execution results (the Virtual Machine Output).
        /// </summary>
        /// <param name="provider">The elrond provider</param>
        /// <param name="address">he Address of the Smart Contract.</param>
        /// <param name="abiDefinition">The smart contract ABI Definition</param>
        /// <param name="endpoint">The name of the Pure Function to execute.</param>
        /// <param name="args">The arguments of the Pure Function. Can be empty</param>
        /// <returns>The response</returns>
        public static Task<IBinaryType> QuerySmartContractWithAbiDefinition(
            IElrondProvider provider,
            AddressValue address,
            AbiDefinition abiDefinition,
            string endpoint,
            params IBinaryType[] args)
        {
            var endpointDefinition = abiDefinition.GetEndpointDefinition(endpoint);
            var outputs = endpointDefinition.Output.Select(o => o.Type).ToArray();
            if (outputs.Length != 1)
                throw new Exception("Bad output quantities in ABI definition. Should only be one.");

            return QuerySmartContract(provider, address, outputs[0], endpoint, args);
        }

        /// <summary>
        /// Allows one to execute - with no side-effects - a pure function of a Smart Contract and retrieve the execution results (the Virtual Machine Output).
        /// </summary>
        /// <param name="provider">The elrond provider</param>
        /// <param name="address">he Address of the Smart Contract.</param>
        /// <param name="outputTypeValue">Output value type of the response</param>
        /// <param name="endpoint">The name of the Pure Function to execute.</param>
        /// <param name="args">The arguments of the Pure Function. Can be empty</param>
        /// <returns>The response</returns>
        public static async Task<IBinaryType> QuerySmartContract(
            IElrondProvider provider,
            AddressValue address,
            TypeValue outputTypeValue,
            string endpoint,
            params IBinaryType[] args)
        {
            var arguments = args
                .Select(typeValue => Convert.ToHexString(BinaryCoder.EncodeTopLevel(typeValue)))
                .ToArray();

            var query = new QueryVmRequestDto
            {
                FuncName = endpoint,
                Args = arguments,
                ScAddress = address.Bech32
            };

            var response = await provider.QueryVm(query);
            var data = response.Data;
            if (data.ReturnData.Length > 1)
            {
                var multiTypes = outputTypeValue.MultiTypes;
                var optional = false;
                if (outputTypeValue.BinaryType == TypeValue.BinaryTypes.Option)
                {
                    optional = true;
                    multiTypes = outputTypeValue.InnerType?.MultiTypes;
                }

                if (multiTypes == null || !multiTypes.Any())
                    throw new Exception("Output type is not a multi type.");

                var decodedValues = new List<IBinaryType>();
                for (var i = 0; i < multiTypes.Length; i++)
                {
                    var decoded =
                        BinaryCoder.DecodeTopLevel(Convert.FromBase64String(data.ReturnData[i]), multiTypes[i]);
                    decodedValues.Add(decoded);
                }

                var multiValue = MultiValue.From(decodedValues.ToArray());
                return optional ? OptionValue.NewProvided(multiValue) : (IBinaryType) multiValue;
            }

            if (data.ReturnData.Length == 0)
            {
                return BinaryCoder.DecodeTopLevel(new byte[0], outputTypeValue);
            }

            var returnData = Convert.FromBase64String(data.ReturnData[0]);
            var decodedResponse = BinaryCoder.DecodeTopLevel(returnData, outputTypeValue);
            return decodedResponse;
        }

        private static byte[] ConcatByteArrays(params byte[][] arrays)
        {
            return arrays.SelectMany(x => x).ToArray();
        }

        private static IEnumerable<byte> CalculateHash(byte[] value)
        {
            var digest = new KeccakDigest(256);
            var output = new byte[digest.GetDigestSize()];
            digest.BlockUpdate(value, 0, value.Length);
            digest.DoFinal(output, 0);
            return output;
        }
    }
}