using Erdcsharp.Domain.Exceptions;

namespace Erdcsharp.Provider.Dtos
{
    /// <summary>
    /// In case of a success, the data field is populated, the error field is empty, while the code field is set to **successful**.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ElrondGatewayResponseDto<T>
    {
        /// <summary>
        /// <see cref="T"/>
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Human-readable description of the issue
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// 'successful' in case of success
        /// </summary>
        public string Code { get; set; }

        public void EnsureSuccessStatusCode()
        {
            if (string.IsNullOrEmpty(Error) && Code == "successful")
                return;

            throw new GatewayException(Error, Code);
        }
    }
}
