namespace Elrond.Dotnet.Sdk.Domain
{
    public class EndpointDefinition
    {
        public string Name { get; }
        public FieldDefinition[] Input { get; }
        public FieldDefinition[] Output { get; }

        public EndpointDefinition(string name, FieldDefinition[] input, FieldDefinition[] output)
        {
            Name = name;
            Input = input;
            Output = output;
        }
    }
}