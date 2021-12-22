namespace Erdcsharp.Domain.Values
{
    public class EnumVariantDefinition
    {
        public string Name         { get; }
        public int    Discriminant { get; }

        public EnumVariantDefinition(string name, int discriminant)
        {
            Name         = name;
            Discriminant = discriminant;
        }
    }
}
