namespace Elrond.Dotnet.Sdk.Provider.Dtos
{
    public class QueryVmResultDataDto
    {
        public QueryVmResultInnerDataDto Data { get; set; }
    }

    public class QueryVmResultInnerDataDto
    {
        public string[] ReturnData { get; set; }
        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
    }


}