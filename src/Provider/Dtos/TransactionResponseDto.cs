namespace Erdcsharp.Provider.Dtos
{
    /// <summary>
    /// Gateway transaction response
    /// </summary>
    public class TransactionResponseData
    {
        public TransactionDto Transaction { get; set; }
    }

    /// <summary>
    /// Transaction detail
    /// </summary>
    public class TransactionDto
    {
        public string                   Type                              { get; set; }
        public long                     Nonce                             { get; set; }
        public long                     Round                             { get; set; }
        public long                     Epoch                             { get; set; }
        public string                   Value                             { get; set; }
        public string                   Receiver                          { get; set; }
        public string                   Sender                            { get; set; }
        public long                     GasPrice                          { get; set; }
        public long                     GasLimit                          { get; set; }
        public string                   Data                              { get; set; }
        public string                   Signature                         { get; set; }
        public long                     SourceShard                       { get; set; }
        public long                     DestinationShard                  { get; set; }
        public long                     BlockNonce                        { get; set; }
        public string                   BlockHash                         { get; set; }
        public long                     NotarizedAtSourceInMetaNonce      { get; set; }
        public string                   NotarizedAtSourceInMetaHash       { get; set; }
        public long                     NotarizedAtDestinationInMetaNonce { get; set; }
        public string                   NotarizedAtDestinationInMetaHash  { get; set; }
        public string                   MiniblockType                     { get; set; }
        public string                   MiniblockHash                     { get; set; }
        public string                   Status                            { get; set; }
        public long                     HyperblockNonce                   { get; set; }
        public string                   HyperblockHash                    { get; set; }
        public SmartContractResultDto[] SmartContractResults              { get; set; }
    }

    public class SmartContractResultDto
    {
        public string Hash           { get; set; }
        public long   Nonce          { get; set; }
        public long   Value          { get; set; }
        public string Receiver       { get; set; }
        public string Sender         { get; set; }
        public string Data           { get; set; }
        public string ReturnMessage  { get; set; }
        public string PrevTxHash     { get; set; }
        public string OriginalTxHash { get; set; }
        public long   GasLimit       { get; set; }
        public long   GasPrice       { get; set; }
        public long   CallType       { get; set; }
    }
}
