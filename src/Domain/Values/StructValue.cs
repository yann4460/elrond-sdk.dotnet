using Elrond.Dotnet.Sdk.Domain.Exceptions;

namespace Elrond.Dotnet.Sdk.Domain.Values
{
    public class StructValue : IBinaryType
    {
        public TypeValue Type { get; }

        public byte[] Buffer => throw new System.NotImplementedException();

        public StructField[] Fields { get; }

        public StructValue(TypeValue structType, StructField[] fields)
        {
            Type = structType;
            Fields = fields;
            CheckTyping();
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

                if (fieldType.RustType != definition.RustType)
                    throw new BinaryCodecException("field rustType vs. field definitions rustType");
            }
        }
    }
}