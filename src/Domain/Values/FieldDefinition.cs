namespace Erdcsharp.Domain.Values
{
    public class FieldDefinition
    {
        public string    Name        { get; }
        public string    Description { get; }
        public TypeValue Type        { get; }

        public FieldDefinition(string name, string description, TypeValue type)
        {
            Name        = name;
            Description = description;
            Type        = type;
            Type.SetName(name);
        }
    }
}
