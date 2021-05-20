namespace Elrond.Dotnet.Sdk.Domain.Values
{
    public class StructField
    {
        public IBinaryType Value { get; }
        public string Name { get; }

        public StructField(IBinaryType value, string name = "")
        {
            Value = value;
            Name = name;
        }
    }
}