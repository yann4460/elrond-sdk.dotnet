using System.Text.Json;

namespace Elrond.Dotnet.Sdk.Domain.Values
{
    public class FieldDefinition
    {
        public string Name { get; }
        public string Description { get; }
        public string RustType { get; }

        public FieldDefinition(string name, string description, string rustType)
        {
            Name = name;
            Description = description;
            RustType = rustType;
        }


        public static FieldDefinition FromJson(string json)
        {
            var dynamic = JsonSerializer.Deserialize<dynamic>(json);

            return new FieldDefinition("", "", "");
        }
    }
}