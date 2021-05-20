using System.Linq;
using System.Text.Json;

namespace Elrond.Dotnet.Sdk.Domain.Values
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

        public static EndpointDefinition FromJson(string name, string json)
        {
            var abi = JsonSerializer.Deserialize<AbiDefinition>(json);

            var endpoint = abi.Endpoints.ToList().SingleOrDefault(e => e.Name == name);

            var inputs = endpoint.Inputs.Select(i => new FieldDefinition(i.Name, "", TypeValue.FromRustType(i.Type)));
            var outputs = endpoint.Outputs.Select(i => new FieldDefinition("", "", TypeValue.FromRustType(i.Type)));

            return new EndpointDefinition(name, inputs.ToArray(), outputs.ToArray());
        }
    }
}