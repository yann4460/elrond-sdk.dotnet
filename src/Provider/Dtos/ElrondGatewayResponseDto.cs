using Elrond.Dotnet.Sdk.Domain.Exceptions;

namespace Elrond.Dotnet.Sdk.Provider.Dtos
{
    public class ElrondGatewayResponseDto<T>
    {
        public T Data { get; set; }
        public string Error { get; set; }
        public string Code { get; set; }

        public void EnsureSuccessStatusCode()
        {
            if (string.IsNullOrEmpty(Error))
                return;

            throw new GatewayException(Error, Code);
        }
    }
}