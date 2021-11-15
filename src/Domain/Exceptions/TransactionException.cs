using System;

namespace Erdcsharp.Domain.Exceptions
{
    public class TransactionException
    {
        public class TransactionStatusNotReachedException : Exception
        {
            public TransactionStatusNotReachedException(string transactionHash, string expectedStatus)
                : base($"Cannot reach {expectedStatus} status for tx : '{transactionHash}'")
            {
            }
        }

        public class InvalidTransactionException : Exception
        {
            public InvalidTransactionException(string transactionHash)
                : base($"Transaction is invalid for tx : '{transactionHash}'")
            {
            }
        }

        public class FailedTransactionException : Exception
        {
            public FailedTransactionException(string transactionHash)
                : base($"Transaction failed for tx : '{transactionHash}'")
            {
            }
        }

        public class TransactionWithSmartContractErrorException : Exception
        {
            public TransactionWithSmartContractErrorException(string transactionHash, string message)
                : base($"Transaction tx : '{transactionHash}' has some error : {message}")
            {
            }
        }
    }
}
