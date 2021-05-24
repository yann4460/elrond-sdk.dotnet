using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Elrond.Dotnet.Sdk.Domain.Values;

namespace Elrond.Dotnet.Sdk.Domain
{
    public class EsdtTokenTransactionRequest
    {
        private static readonly AddressValue EsdtNftAddress = AddressValue.FromBech32("erd1qqqqqqqqqqqqqqqpqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqzllls8a5w6u");

        private const string Issue = "issue";
        private const string SetSpecialRole = "setSpecialRole";
        private const string IssueSemiFungible = "issueSemiFungible";
        private const string IssueNonFungible = "issueNonFungible";
        private const string EsdtNftTransfer = "ESDTNFTTransfer";
        private const string EsdtTransfer = "ESDTTransfer";
        private const string EsdtNftCreate = "ESDTNFTCreate";

        public static class NFTRoles
        {
            /// <summary>
            /// This role allows one to create a new NFT
            /// </summary>
            public const string ESDTRoleNFTCreate = "ESDTRoleNFTCreate";

            /// <summary>
            /// This role allows one to burn quantity of a specific NFT
            /// </summary>
            public const string ESDTRoleNFTBurn = "ESDTRoleNFTBurn";
        }

        public static class SFTRoles
        {
            /// <summary>
            /// This role allows one to create a new SFT
            /// </summary>
            public const string ESDTRoleNFTCreate = "ESDTRoleNFTCreate";

            /// <summary>
            /// This role allows one to burn quantity of a specific SFT
            /// </summary>
            public const string ESDTRoleNFTBurn = "ESDTRoleNFTBurn";

            /// <summary>
            /// This role allows one to add quantity of a specific SFT
            /// </summary>
            public const string ESDTRoleNFTAddQuantity = "ESDTRoleNFTAddQuantity";
        }

        /// <summary>
        /// Issue an ESDT Token
        /// </summary>
        /// <param name="constants"></param>
        /// <param name="account"></param>
        /// <param name="tokenName">The token name, length between 3 and 20 characters (alphanumeric characters only)</param>
        /// <param name="tokenTicker">The token ticker, length between 3 and 10 characters (alphanumeric UPPERCASE only)</param>
        /// <param name="initialSupply">The initial supply</param>
        /// <param name="numberOfDecimals">Number of decimals, should be a numerical value between 0 and 18</param>
        /// <returns>The transaction request</returns>
        public static TransactionRequest IssueEsdtTransactionRequest(
            Constants constants,
            Account account,
            string tokenName,
            string tokenTicker,
            BigInteger initialSupply,
            ushort numberOfDecimals)
        {
            var balance = constants.ChainId == "T" ? Balance.EGLD("5") : Balance.EGLD("0.05");
            var transaction = SmartContract.CreateCallSmartContractTransactionRequest(constants,
                account,
                EsdtNftAddress,
                Issue,
                balance,
                BytesValue.FromUtf8(tokenName),
                BytesValue.FromUtf8(tokenTicker),
                NumericValue.BigUintValue(initialSupply),
                NumericValue.U16Value(numberOfDecimals));

            transaction.SetGasLimit(new GasLimit(60000000));

            return transaction;
        }

        /// <summary>
        /// In order to be able to perform actions over a token, one needs to have roles assigned.
        /// </summary>
        /// <param name="constants"></param>
        /// <param name="account"></param>
        /// <param name="receiver"></param>
        /// <param name="tokenIdentifier"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public static TransactionRequest SetSpecialRoleTransactionRequest(
            Constants constants,
            Account account,
            AddressValue receiver,
            string tokenIdentifier,
            params string[] roles)
        {
            var transaction = SmartContract.CreateCallSmartContractTransactionRequest(
                constants,
                account,
                EsdtNftAddress,
                SetSpecialRole,
                Balance.Zero(),
                TokenIdentifierValue.From(tokenIdentifier),
                receiver);

            transaction.AddArgument(roles.Select<string, IBinaryType>(BytesValue.FromUtf8).ToArray());
            transaction.SetGasLimit(new GasLimit(60000000));

            return transaction;
        }

        /// <summary>
        /// Issue a Non fungible token
        /// </summary>
        /// <param name="constants"></param>
        /// <param name="account"></param>
        /// <param name="tokenName">The token name, length between 3 and 20 characters (alphanumeric characters only)</param>
        /// <param name="tokenTicker">The token ticker, length between 3 and 10 characters (alphanumeric UPPERCASE only)</param>
        /// <returns>The transaction request</returns>
        public static TransactionRequest IssueNonFungibleTokenTransactionRequest(
            Constants constants,
            Account account,
            string tokenName,
            string tokenTicker)
        {
            var balance = constants.ChainId == "T" ? Balance.EGLD("5") : Balance.EGLD("0.05");
            var transaction = SmartContract.CreateCallSmartContractTransactionRequest(constants,
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
        /// <param name="constants"></param>
        /// <param name="account"></param>
        /// <param name="tokenName">The token name, length between 3 and 20 characters (alphanumeric characters only)</param>
        /// <param name="tokenTicker">The token ticker, length between 3 and 10 characters (alphanumeric UPPERCASE only)</param>
        /// <returns>The transaction request</returns>
        public static TransactionRequest IssueSemiFungibleTokenTransactionRequest(
            Constants constants,
            Account account,
            string tokenName,
            string tokenTicker)
        {
            var balance = constants.ChainId == "T" ? Balance.EGLD("5") : Balance.EGLD("0.05");
            var transaction = SmartContract.CreateCallSmartContractTransactionRequest(constants,
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
        /// <param name="constants"></param>
        /// <param name="account"></param>
        /// <param name="receiver">The destination address</param>
        /// <param name="tokenIdentifier">The token identifier</param>
        /// <param name="tokenId">The nonce after the NFT creation</param>
        /// <param name="quantity">Should be 1 if NFT</param>
        /// <returns>The transaction request</returns>
        public static TransactionRequest TransferEsdtNftTransactionRequest(
            Constants constants,
            Account account,
            AddressValue receiver,
            string tokenIdentifier,
            ulong tokenId,
            ulong quantity)
        {
            var transaction = SmartContract.CreateCallSmartContractTransactionRequest(constants,
                account,
                account.Address,
                EsdtNftTransfer,
                Balance.Zero(),
                TokenIdentifierValue.From(tokenIdentifier),
                NumericValue.U64Value(tokenId),
                NumericValue.U64Value(quantity),
                receiver
            );

            //GasLimit: 1000000 + length of Data field in bytes * 1500
            transaction.SetGasLimit(new GasLimit(1000000));

            return transaction;
        }

        /// <summary>
        /// Perform a ESDT Transfer
        /// </summary>
        /// <param name="constants"></param>
        /// <param name="account"></param>
        /// <param name="receiver">Destination address</param>
        /// <param name="tokenIdentifier">The token identifier</param>
        /// <param name="quantity">Quantity to transfer</param>
        /// <returns>The transaction request</returns>
        public static TransactionRequest TransferEsdtTransactionRequest(
            Constants constants,
            Account account,
            AddressValue receiver,
            string tokenIdentifier,
            ulong quantity)
        {
            var transaction = SmartContract.CreateCallSmartContractTransactionRequest(
                constants,
                account,
                receiver,
                EsdtTransfer,
                Balance.Zero(),
                TokenIdentifierValue.From(tokenIdentifier),
                NumericValue.U64Value(quantity));

            transaction.SetGasLimit(new GasLimit(500000));

            return transaction;
        }

        /// <summary>
        /// Create a NFT token
        /// </summary>
        /// <param name="constants"></param>
        /// <param name="account">Account with ESDTRoleNFTCreate role</param>
        /// <param name="tokenIdentifier">The token identifier</param>
        /// <param name="name">The name of the NFT or SFT</param>
        /// <param name="royalties">Allows the creator to receive royalties for any transaction involving their NFT (Base format is a numeric value between 0 an 10000 (0 meaning 0% and 10000 meaning 100%)</param>
        /// <param name="hash">Arbitrary field that should contain the hash of the NFT metadata.</param>
        /// <param name="attributes">Arbitrary field that should contain a set of attributes in the format desired by the creator</param>
        /// <param name="uris">Minimum one field that should contain the Uniform Resource Identifier. Can be a URL to a media file or something similar.</param>
        /// <returns></returns>
        public static TransactionRequest CreateEsdtNftTokenTransactionRequest(
            Constants constants,
            Account account,
            string tokenIdentifier,
            string name,
            ushort royalties,
            string hash,
            Dictionary<string, string> attributes,
            Uri[] uris)
        {
            if (royalties > 10000)
                throw new ArgumentException("Value should be between 0 an 10000 (0 meaning 0% and 10000 meaning 100%",
                    nameof(royalties));

            if (!string.IsNullOrEmpty(hash) && Encoding.UTF8.GetBytes(hash).Length != 32)
                throw new ArgumentException("Hash should be a 32 bytes length H256", nameof(hash));

            if (uris.Length == 0)
                throw new ArgumentException("At least one URI should be provided", nameof(uris));

            var attributeValue = string.Join(";", attributes.Select(x => x.Key + ":" + x.Value).ToArray());
            var urisValue = uris.Select(u => (IBinaryType) BytesValue.FromUtf8(u.AbsoluteUri)).ToArray();
            var transaction = SmartContract.CreateCallSmartContractTransactionRequest(
                constants,
                account,
                account.Address,
                EsdtNftCreate,
                Balance.Zero(),
                TokenIdentifierValue.From(tokenIdentifier),
                NumericValue.BigUintValue(1),
                BytesValue.FromUtf8(name),
                NumericValue.U16Value(royalties),
                string.IsNullOrEmpty(hash)
                    ? OptionValue.NewMissing()
                    : OptionValue.NewProvided(BytesValue.FromUtf8(hash)),
                string.IsNullOrEmpty(attributeValue)
                    ? OptionValue.NewMissing()
                    : OptionValue.NewProvided(BytesValue.FromUtf8(attributeValue)));

            transaction.AddArgument(urisValue);

            const int storePerByte = 50000;
            // Transaction payload cost: Data field length * 1500 (GasPerDataByte = 1500)
            var transactionCost = Convert.FromBase64String(transaction.Data).Length * constants.GasPerDataByte;
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