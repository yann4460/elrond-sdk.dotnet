using System.Linq;

namespace Erdcsharp.Domain.Values
{
    public class EnumValue : BaseBinaryValue
    {
        public EnumVariantDefinition Variant { get; }

        public EnumValue(TypeValue structType, EnumVariantDefinition variant) : base(structType)
        {
            Variant = variant;
        }

        public static EnumValue FromName(TypeValue type, string name)
        {
            var variant = type.GetVariantByName(name);
            return new EnumValue(type, variant);
        }

        public static EnumValue FromDiscriminant(TypeValue type, int discriminant)
        {
            var variant = type.GetVariantByDiscriminant(discriminant);
            return new EnumValue(type, variant);
        }

        public override string ToString()
        {
            return this.Variant.Name;
        }
    }
}
