namespace Elrond.Dotnet.Sdk.Domain.Values
{
    public interface IBinaryType
    {
        TypeValue Type { get; }

        byte[] Buffer { get; }

        T ValueOf<T>() where T : IBinaryType
        {
            return (T) this;
        }
    }
}