using System;
using System.Collections.Generic;
using System.Linq;

namespace Elrond.Dotnet.Sdk.Domain.Values
{
    public class AbiDefinition
    {
        public string Name { get; set; }
        public Constructor Constructor { get; set; }
        public Endpoint[] Endpoints { get; set; }
        public Dictionary<string, Types> Types { get; set; }

        public EndpointDefinition GetEndpointDefinition(string endpoint)
        {
            var data = Endpoints.ToList().SingleOrDefault(s => s.Name == endpoint);
            if (data == null)
                throw new Exception("Endpoint is not define in ABI");

            var inputs = data.Inputs.Select(i =>
                new FieldDefinition(i.Name, "", TypeValue.FromRustType(i.Type) ?? GetFromAbiTypes(i.Type)));
            var outputs = data.Outputs.Select(i =>
                new FieldDefinition("", "", TypeValue.FromRustType(i.Type) ?? GetFromAbiTypes(i.Type)));
            return new EndpointDefinition(endpoint, inputs.ToArray(), outputs.ToArray());
        }

        private TypeValue GetFromAbiTypes(string name)
        {
            name = name.Replace("optional", "").Replace("<", "").Replace(">", "");
            var type = Types[name];

            return TypeValue.StructValue(type.Type,
                type.Fields.ToList().Select(c =>
                        new FieldDefinition(c.Name, "", TypeValue.FromRustType(c.Type) ?? GetFromAbiTypes(c.Type)))
                    .ToArray());
        }
    }

    public class Constructor
    {
        public Input[] Inputs { get; set; }
        public object[] Outputs { get; set; }
    }

    public class Input
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool multi_arg { get; set; }
    }

    public class Types
    {
        public string Type { get; set; }
        public Field[] Fields { get; set; }
    }

    public class Field
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class Endpoint
    {
        public string Name { get; set; }
        public Input[] Inputs { get; set; }
        public Output[] Outputs { get; set; }
        public string[] PayableInTokens { get; set; }
    }

    public class Output
    {
        public string Type { get; set; }
        public bool MultiResult { get; set; }
    }
}