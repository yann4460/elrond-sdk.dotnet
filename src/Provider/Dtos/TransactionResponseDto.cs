namespace Elrond.Dotnet.Sdk.Provider.Dtos
{
    public class TransactionResponseDto
    {
        public string TxHash { get; set; }
        public string Data { get; set; }
        public string Fee { get; set; }
        public long GasLimit { get; set; }
        public long GasPrice { get; set; }
        public long GasUsed { get; set; }
        public string MiniBlockHash { get; set; }
        public long Nonce { get; set; }
        public string Receiver { get; set; }
        public long ReceiverShard { get; set; }
        public long Round { get; set; }
        public ScResult[] ScResults { get; set; }
        public string Sender { get; set; }
        public long SenderShard { get; set; }
        public string Signature { get; set; }
        public string Status { get; set; }
        public long Timestamp { get; set; }
        public string Value { get; set; }
        
        public class ScResult
        {
            public string RelayedValue { get; set; }
            public string PrevTxHash { get; set; }
            public long GasLimit { get; set; }
            public string OriginalTxHash { get; set; }
            public string Receiver { get; set; }
            public string Data { get; set; }
            public string Sender { get; set; }
            public long Nonce { get; set; }
            public string Value { get; set; }
            public string Hash { get; set; }
            public string CallType { get; set; }
            public long GasPrice { get; set; }
        }
    }
}