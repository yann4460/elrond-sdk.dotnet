using Elrond.Dotnet.Sdk.Domain.Exceptions;

namespace Elrond.Dotnet.Sdk.Domain.Codec
{
    public class StructValue : IBinaryType
    {
        public string Name { get; }
        private readonly FieldDefinition[] _fields;

        public StructValue(string name, FieldDefinition[] fields)
        {
            Name = name;
            _fields = fields;
            //CheckTyping();
        }

        //private void CheckTyping()
        //{
        //    var definitions = this._type.GetFields();
        //    if (_fields.Length != definitions.Length)
        //    {
        //        throw new BinaryCodecException("fields length vs. field definitions length");
        //    }

        //    for (var i = 0; i < _fields.Length; i++)
        //    {
        //        var field = _fields[i];
        //        var definition = definitions[i];
        //        var fieldType = field.Type;

        //        if (fieldType.Name != definition.Type.Name)
        //            throw new BinaryCodecException("field name vs. field definitions name");
        //    }
        //}
        public TypeValue Type => TypeValue.Struct;

        public IBinaryType ValueOf()
        {
            return this;
        }
    }
}