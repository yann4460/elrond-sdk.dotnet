﻿using System.Linq;
using System.Text;
using System.Text.Json;
using Elrond.Dotnet.Sdk.Domain.Exceptions;

namespace Elrond.Dotnet.Sdk.Domain.Values
{
    public class StructValue : IBinaryType
    {
        public TypeValue Type { get; }
        public StructField[] Fields { get; }

        public StructValue(TypeValue structType, StructField[] fields)
        {
            Type = structType;
            Fields = fields;
            CheckTyping();
        }

        public StructField GetStructField(string name)
        {
            var field = Fields.SingleOrDefault(f => f.Name == name);
            return field;
        }

        private void CheckTyping()
        {
            var definitions = Type.GetFieldDefinitions();
            if (Fields.Length != definitions.Length)
            {
                throw new BinaryCodecException("fields length vs. field definitions length");
            }

            for (var i = 0; i < Fields.Length; i++)
            {
                var field = Fields[i];
                var definition = definitions[i];
                var fieldType = field.Value.Type;

                if (fieldType.RustType != definition.Type.RustType)
                    throw new BinaryCodecException("field rustType vs. field definitions rustType");
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine(Type.Name);
            foreach (var structField in Fields)
            {
                builder.AppendLine($"{structField.Name}:{structField.Value}");
            }

            return builder.ToString();
        }

        public string ToJSON()
        {
            var dic = Fields.ToDictionary(s => s.Name, s => s.Value.ToJSON());
            return JsonSerializer.Serialize(dic);
        }
    }
}