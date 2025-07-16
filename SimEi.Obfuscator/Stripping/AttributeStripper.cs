using AsmResolver.DotNet;
using SimEi.Obfuscator.Renaming;

namespace SimEi.Obfuscator.Stripping
{
    internal class AttributeStripper : ModuleVisitorBase
    {
        private readonly Predicate<ITypeDefOrRef> _shouldStripAttribute;

        public AttributeStripper(Predicate<ITypeDefOrRef> shouldStripAttribute)
        {
            _shouldStripAttribute = shouldStripAttribute;
        }


        public override void VisitType(TypeDefinition type) => StripAttributes(type.CustomAttributes);
        public override void VisitField(FieldDefinition field) => StripAttributes(field.CustomAttributes);
        public override void VisitProp(PropertyDefinition prop) => StripAttributes(prop.CustomAttributes);
        public override void VisitEvent(EventDefinition evt) => StripAttributes(evt.CustomAttributes);

        public override void VisitMethod(MethodDefinition method)
        {
            StripAttributes(method.CustomAttributes);
            foreach (var param in method.Parameters)
            {
                if (param.Definition != null)
                    StripAttributes(param.Definition.CustomAttributes);
            }
        }

        private void StripAttributes(IList<CustomAttribute> attributes)
        {
            for (int i = attributes.Count - 1; i >= 0; i--)
            {
                if (_shouldStripAttribute.Invoke(attributes[i].Constructor!.DeclaringType!))
                    attributes.RemoveAt(i);
            }
        }
    }
}
