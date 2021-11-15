namespace Erdcsharp.Provider.Dtos
{
    public class TransactionRequestDto
    {
        public long   Nonce     { get; set; }
        public string Value     { get; set; }
        public string Receiver  { get; set; }
        public string Sender    { get; set; }
        public long   GasPrice  { get; set; }
        public long   GasLimit  { get; set; }
        public string Data      { get; set; }
        public string ChainID   { get; set; }
        public int    Version   { get; set; }
        public string Signature { get; set; }
    }
}
