namespace Elrond.Dotnet.Sdk.Domain.Values
{
    public interface IBinaryType
    {
        TypeValue Type { get; }

        T ValueOf<T>() where T : IBinaryType
        {
            return (T) this;
        }
    }
}