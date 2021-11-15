namespace Erdcsharp.Provider.Dtos
{
    public class QueryVmRequestDto
    {
        /// <summary>
        /// The Address (bech32) of the Smart Contract.
        /// </summary>
        public string ScAddress { get; set; }

        /// <summary>
        /// The rustType of the Pure Function to execute.
        /// </summary>
        public string FuncName { get; set; }

        /// <summary>
        /// The arguments of the Pure Function, as hex-encoded strings. The array can be empty.
        /// </summary>
        public string[] Args { get; set; }

        /// <summary>
        /// The Address (bech32) of the caller.
        /// </summary>
        public string Caller { get; set; }

        /// <summary>
        /// The Value to transfer (can be zero).
        /// </summary>
        public string value { get; set; }
    }
}
