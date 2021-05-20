namespace Elrond.Dotnet.Sdk.Domain.Codec
{
    public interface IBinaryType
    {
        TypeValue Type { get; }

        IBinaryType ValueOf();
    }
}