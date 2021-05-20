using Elrond.Dotnet.Sdk.Domain.Exceptions;

namespace Elrond.Dotnet.Sdk.Domain.Values
{
    public class StructValue : IBinaryType
    {
        public TypeValue Type { get; }
        private readonly StructField[] _fields;

        public StructValue(TypeValue structType, StructField[] fields)
        {
            Type = structType;
            _fields = fields;
            CheckTyping();
        }

        private void CheckTyping()
        {
            var definitions = Type.GetFieldDefinitions();
            if (_fields.Length != definitions.Length)
            {
                throw new BinaryCodecException("fields length vs. field definitions length");
            }

            for (var i = 0; i < _fields.Length; i++)
            {
                var field = _fields[i];
                var definition = definitions[i];
                var fieldType = field.Value.Type;

                if (fieldType.RustType != definition.RustType)
                    throw new BinaryCodecException("field rustType vs. field definitions rustType");
            }
        }
    }
}