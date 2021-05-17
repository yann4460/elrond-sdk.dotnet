namespace Elrond.Dotnet.Sdk.Provider.Dtos
{
    public class QueryVmResultDto
    {
        public QueryVmResultDataDto Data { get; set; }
        public string Error { get; set; }
        public string Code { get; set; }
    }

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