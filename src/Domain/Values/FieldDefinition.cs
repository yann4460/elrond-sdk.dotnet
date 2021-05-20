﻿namespace Elrond.Dotnet.Sdk.Domain.Values
{
    public class FieldDefinition
    {
        public string Name { get; }
        public string Description { get; }
        public TypeValue Type { get; }

        public FieldDefinition(string name, string description, TypeValue type)
        {
            Name = name;
            Description = description;
            Type = type;
        }

        //public static FieldDefinition FromJson(string json)
        //{
        //    var dynamic = JsonSerializer.Deserialize<dynamic>(json);

        //    return new FieldDefinition("", "", "");
        //}
    }
}