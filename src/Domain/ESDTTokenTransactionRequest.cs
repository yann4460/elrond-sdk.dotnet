using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Erdcsharp.Domain.Values;

namespace Erdcsharp.Domain
{
    public class EsdtTokenTransactionRequest
    {
        private static readonly Address EsdtNftAddress = Address.FromBech32(Constants.SmartContractAddress.EsdtSmartContract);

        private const string Issue             = "issue";
        private const string IssueSemiFungible = "issueSemiFungible";
        private const string IssueNonFungible  = "issueNonFungible";
        private const string SetSpecialRole    = "setSpecialRole";
        private const string EsdtNftTransfer   = "ESDTNFTTransfer";
        private const string EsdtNftCreate     = "ESDTNFTCreate";
        private const string EsdtTransfer      = "ESDTTransfer";


        /// <summary>
        /// Issue an FungibleESDT Token
        /// </summary>
        /// <param name="networkConfig"></param>
        /// <param name="account"></param>
        /// <param name="tokenName">The token name, length between 3 and 20 characters (alphanumeric characters only)</param>
        /// <param name="tokenTicker">The token ticker, length between 3 and 10 characters (alphanumeric UPPERCASE only)</param>
        /// <param name="initialSupply">The initial supply</param>
        /// <param name="numberOfDecimals">Number of decimals, should be a numerical value between 0 and 18</param>
        /// <returns>The transaction request</returns>
        public static TransactionRequest IssueEsdtTransactionRequest(
            NetworkConfig networkConfig,
            Account account,
            string tokenName,
            string tokenTicker,
            BigInteger initialSupply,
            int numberOfDecimals)
        {
            var balance = TokenAmount.EGLD("0.05");
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           EsdtNftAddress,
                                                                                           Issue,
                                                                                           balance,
                                                                                           BytesValue.FromUtf8(tokenName),
                                                                                           BytesValue.FromUtf8(tokenTicker),
                                                                                           NumericValue.BigUintValue(initialSupply),
                                                                                           NumericValue.I32Value(numberOfDecimals));

            transaction.SetGasLimit(new GasLimit(60000000));

            return transaction;
        }

        /// <summary>
        /// In order to be able to perform actions over a token, one needs to have roles assigned.
        /// </summary>
        /// <param name="networkConfig"></param>
        /// <param name="account"></param>
        /// <param name="receiver"></param>
        /// <param name="tokenIdentifier"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public static TransactionRequest SetSpecialRoleTransactionRequest(
            NetworkConfig networkConfig,
            Account account,
            Address receiver,
            string tokenIdentifier,
            params string[] roles)
        {
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(
                                                                                           networkConfig,
                                                                                           account,
                                                                                           EsdtNftAddress,
                                                                                           SetSpecialRole,
                                                                                           TokenAmount.Zero(),
                                                                                           TokenIdentifierValue.From(tokenIdentifier),
                                                                                           receiver);

            transaction.AddArgument(roles.Select<string, IBinaryType>(BytesValue.FromUtf8).ToArray());
            transaction.SetGasLimit(new GasLimit(60000000));

            return transaction;
        }

        /// <summary>
        /// Issue a Non fungible token
        /// </summary>
        /// <param name="networkConfig"></param>
        /// <param name="account"></param>
        /// <param name="tokenName">The token name, length between 3 and 20 characters (alphanumeric characters only)</param>
        /// <param name="tokenTicker">The token ticker, length between 3 and 10 characters (alphanumeric UPPERCASE only)</param>
        /// <returns>The transaction request</returns>
        public static TransactionRequest IssueNonFungibleTokenTransactionRequest(
            NetworkConfig networkConfig,
            Account account,
            string tokenName,
            string tokenTicker)
        {
            var balance = TokenAmount.EGLD("0.05");
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           EsdtNftAddress,
                                                                                           IssueNonFungible,
                                                                                           balance,
                                                                                           BytesValue.FromUtf8(tokenName),
                                                                                           BytesValue.FromUtf8(tokenTicker));

            transaction.SetGasLimit(new GasLimit(60000000));

            return transaction;
        }

        /// <summary>
        /// Issue a Semi fungible token
        /// </summary>
        /// <param name="networkConfig"></param>
        /// <param name="account"></param>
        /// <param name="tokenName">The token name, length between 3 and 20 characters (alphanumeric characters only)</param>
        /// <param name="tokenTicker">The token ticker, length between 3 and 10 characters (alphanumeric UPPERCASE only)</param>
        /// <returns>The transaction request</returns>
        public static TransactionRequest IssueSemiFungibleTokenTransactionRequest(
            NetworkConfig networkConfig,
            Account account,
            string tokenName,
            string tokenTicker)
        {
            var balance = TokenAmount.EGLD("0.05");
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           EsdtNftAddress,
                                                                                           IssueSemiFungible,
                                                                                           balance,
                                                                                           BytesValue.FromUtf8(tokenName),
                                                                                           BytesValue.FromUtf8(tokenTicker));

            transaction.SetGasLimit(new GasLimit(60000000));

            return transaction;
        }

        /// <summary>
        /// Perform a ESDTNFT Transfer
        /// </summary>
        /// <param name="networkConfig"></param>
        /// <param name="account"></param>
        /// <param name="receiver">The destination address</param>
        /// <param name="tokenIdentifier">The token identifier</param>
        /// <param name="tokenId">The nonce after the NFT creation</param>
        /// <param name="quantity">Should be 1 if NFT</param>
        /// <returns>The transaction request</returns>
        public static TransactionRequest TransferEsdtNftTransactionRequest(
            NetworkConfig networkConfig,
            Account account,
            Address receiver,
            string tokenIdentifier,
            BigInteger tokenId,
            BigInteger quantity)
        {
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           account.Address,
                                                                                           EsdtNftTransfer,
                                                                                           TokenAmount.Zero(),
                                                                                           TokenIdentifierValue.From(tokenIdentifier),
                                                                                           NumericValue.BigUintValue(tokenId),
                                                                                           NumericValue.BigUintValue(quantity),
                                                                                           receiver
                                                                                          );

            //GasLimit: 1000000 + length of Data field in bytes * 1500
            transaction.SetGasLimit(new GasLimit(1000000));

            return transaction;
        }

        /// <summary>
        /// Perform a FungibleESDT Transfer
        /// </summary>
        /// <param name="networkConfig"></param>
        /// <param name="account"></param>
        /// <param name="receiver">Destination address</param>
        /// <param name="tokenIdentifier">The token identifier</param>
        /// <param name="quantity">Quantity to transfer</param>
        /// <returns>The transaction request</returns>
        public static TransactionRequest TransferEsdtTransactionRequest(
            NetworkConfig networkConfig,
            Account account,
            Address receiver,
            string tokenIdentifier,
            BigInteger quantity)
        {
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(
                                                                                           networkConfig,
                                                                                           account,
                                                                                           receiver,
                                                                                           EsdtTransfer,
                                                                                           TokenAmount.Zero(),
                                                                                           TokenIdentifierValue.From(tokenIdentifier),
                                                                                           NumericValue.BigIntValue(quantity));

            transaction.SetGasLimit(new GasLimit(500000));

            return transaction;
        }

        /// <summary>
        /// Create a NFT token
        /// </summary>
        /// <param name="networkConfig"></param>
        /// <param name="account">Account with ESDTRoleNFTCreate role</param>
        /// <param name="tokenIdentifier">The token identifier</param>
        /// <param name="quantity">Should be one if NFT</param>
        /// <param name="name">The name of the NFT or SemiFungible</param>
        /// <param name="royalties">Allows the creator to receive royalties for any transaction involving their NFT (Base format is a numeric value between 0 an 10000 (0 meaning 0% and 10000 meaning 100%)</param>
        /// <param name="hash">Arbitrary field that should contain the hash of the NFT metadata.</param>
        /// <param name="attributes">Arbitrary field that should contain a set of attributes in the format desired by the creator</param>
        /// <param name="uris">Minimum one field that should contain the Uniform Resource Identifier. Can be a URL to a media file or something similar.</param>
        /// <returns></returns>
        public static TransactionRequest CreateEsdtNftTokenTransactionRequest(
            NetworkConfig networkConfig,
            Account account,
            string tokenIdentifier,
            BigInteger quantity,
            string name,
            ushort royalties,
            byte[] hash,
            Dictionary<string, string> attributes,
            Uri[] uris)
        {
            if (royalties > 10000)
                throw new ArgumentException("Value should be between 0 an 10000 (0 meaning 0% and 10000 meaning 100%",
                                            nameof(royalties));

            if (uris.Length == 0)
                throw new ArgumentException("At least one URI should be provided", nameof(uris));

            var attributeValue = string.Join(";", attributes.Select(x => x.Key + ":" + x.Value).ToArray());
            var urisValue      = uris.Select(u => (IBinaryType)BytesValue.FromUtf8(u.AbsoluteUri)).ToArray();
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(
                                                                                           networkConfig,
                                                                                           account,
                                                                                           account.Address,
                                                                                           EsdtNftCreate,
                                                                                           TokenAmount.Zero(),
                                                                                           TokenIdentifierValue.From(tokenIdentifier),
                                                                                           NumericValue.BigUintValue(quantity),
                                                                                           TokenIdentifierValue.From(name),
                                                                                           NumericValue.U16Value(royalties),
                                                                                           BytesValue.FromBuffer(hash ?? new byte[0]),
                                                                                           BytesValue.FromUtf8(attributeValue));

            transaction.AddArgument(urisValue);

            const int storePerByte = 50000;
            // Transaction payload cost: Data field length * 1500 (GasPerDataByte = 1500)
            var transactionCost = Convert.FromBase64String(transaction.Data).Length * networkConfig.GasPerDataByte;
            // Storage cost: Size of NFT data * 50000 (StorePerByte = 50000)
            var storageCost = (string.IsNullOrEmpty(attributeValue)
                                  ? 0
                                  : storePerByte * BytesValue.FromUtf8(attributeValue).GetLength()) +
                              storePerByte * urisValue.Sum(u => u.ValueOf<BytesValue>().GetLength());
            transaction.SetGasLimit(new GasLimit(6000000 + transactionCost + storageCost));

            return transaction;
        }
    }
}
