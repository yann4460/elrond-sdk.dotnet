using System;
using System.Linq;
using Org.BouncyCastle.Crypto.Digests;
using Sdk.Cryptography;

namespace Sdk.Domain
{
    public class SmartContract
    {
        private const string ArwenVirtualMachine = "0500";

        public static TransactionRequest CreateDeploySmartContractTransactionRequest(
            Constants constants,
            Account account,
            Code code,
            CodeMetadata codeMetadata,
            Argument[] args)
        {
            var transaction = TransactionRequest.CreateTransaction(account, constants);
            var data = $"{code.Value}@{ArwenVirtualMachine}@{codeMetadata.Value}";
            if (args != null)
            {
                data = args?.Aggregate(data, (current, argument) => current + $"@{argument.Value}");
            }

            transaction.SetData(data);

            return transaction;
        }

        public static TransactionRequest CreateUpdateSmartContractTransactionRequest(Constants constants, Account account, Address smartContractAddress)
        {
            var transaction = TransactionRequest.CreateTransaction(account, constants);
            return transaction;
        }

        public static TransactionRequest CreateCallSmartContractTransactionRequest(Constants constants, Account account,
            Address smartContractAddress,
            string functionName,
            Balance value,
            Argument[] args = null)
        {
            var transaction = TransactionRequest.CreateTransaction(account, constants, smartContractAddress, value);
            var data = $"{functionName}";
            if (args != null)
            {
                data = args.Aggregate(data, (current, argument) => current + $"@{argument.Value}");
            }

            transaction.SetData(data);

            return transaction;
        }

        /// <summary>
        /// Computes the address of a Smart Contract.
        /// The address is computed deterministically, from the address of the owner and the nonce of the deployment transaction.
        /// </summary>
        /// <param name="ownerAddress">The owner of the Smart Contract</param>
        /// <param name="nonce">The owner nonce used for the deployment transaction</param>
        /// <returns>The smart contract address</returns>
        public static string ComputeAddress(Address ownerAddress, long nonce)
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
            return erdAddress;
        }

        private static byte[] ConcatByteArrays(params byte[][] arrays)
        {
            return arrays.SelectMany(x => x).ToArray();
        }

        private static byte[] CalculateHash(byte[] value)
        {
            var digest = new KeccakDigest(256);
            var output = new byte[digest.GetDigestSize()];
            digest.BlockUpdate(value, 0, value.Length);
            digest.DoFinal(output, 0);
            return output;
        }
    }
}