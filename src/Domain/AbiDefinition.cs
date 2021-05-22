using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Elrond.Dotnet.Sdk.Domain.Values;

namespace Elrond.Dotnet.Sdk.Domain
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

            var inputs = data.Inputs.Select(i => new FieldDefinition(i.Name, "", GetTypeValue(i.Type))).ToList();
            var outputs = data.Outputs.Select(i => new FieldDefinition("", "", GetTypeValue(i.Type))).ToList();
            return new EndpointDefinition(endpoint, inputs.ToArray(), outputs.ToArray());
        }

        private TypeValue GetTypeValue(string rustType)
        {
            if (rustType.StartsWith("optional"))
            {
                var innerType = rustType.Replace("optional", "").Replace("<", "").Replace(">", "");
                var innerTypeValue = GetTypeValue(innerType);
                return TypeValue.OptionValue(innerTypeValue);
            }

            var typeFromBaseRustType = TypeValue.FromRustType(rustType);
            if (typeFromBaseRustType != null)
                return typeFromBaseRustType;

            if (Types.Keys.Contains(rustType))
            {
                var typeFromStruct = Types[rustType];
                return TypeValue.StructValue(
                    typeFromStruct.Type,
                    typeFromStruct.Fields
                        .ToList()
                        .Select(c => new FieldDefinition(c.Name, "", GetTypeValue(c.Type)))
                        .ToArray()
                );
            }

            return null;
        }

        public static AbiDefinition FromJson(string json)
        {
            var jsonSerializerOptions = new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
            return JsonSerializer.Deserialize<AbiDefinition>(json, jsonSerializerOptions);
        }

        public static async Task<AbiDefinition> FromJsonFilePath(string jsonFilePath)
        {
            var fileBytes = await File.ReadAllBytesAsync(jsonFilePath);
            var json = Encoding.UTF8.GetString(fileBytes);
            return FromJson(json);
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