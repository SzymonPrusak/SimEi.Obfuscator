using AsmResolver.DotNet;

namespace SimEi.Obfuscator.Renaming.Permission
{
    internal class ObfuscationAttributePermissions : ModuleVisitorBase, IRenamingPermissions
    {
        private readonly Dictionary<IMetadataMember, bool> _excluded;

        public ObfuscationAttributePermissions()
        {
            _excluded = new Dictionary<IMetadataMember, bool>();
        }



        public bool CanRename(IMetadataMember member) => !_excluded.ContainsKey(member);


        public override void VisitType(TypeDefinition type)
        {
            VisitMember(type, type.DeclaringType);
        }

        public override void VisitField(FieldDefinition field)
        {
            VisitMember(field, field.DeclaringType);
        }

        public override void VisitProp(PropertyDefinition prop)
        {
            VisitMember(prop, prop.DeclaringType);
        }

        public override void VisitEvent(EventDefinition evt)
        {
            VisitMember(evt, evt.DeclaringType);
        }

        public override void VisitMethod(MethodDefinition method)
        {
            VisitMember(method, method.DeclaringType);
            foreach (var param in method.ParameterDefinitions)
                VisitMember(param, method);
        }


        private void VisitMember(IHasCustomAttribute member, IMetadataMember? parent)
        {
            var entry = CheckExcludeAttribute(member);
            if (!entry.HasValue)
            {
                if (parent != null && DoesPropagateExclusion(parent))
                    _excluded[member] = true;
            }
            else
            {
                (bool exclude, bool applyToMembers) = entry.Value;
                if (exclude)
                    _excluded[member] = applyToMembers;
            }
        }

        private (bool, bool)? CheckExcludeAttribute(IHasCustomAttribute member)
        {
            var obfAttributes = member.CustomAttributes
                .Where(c => c.Constructor!.DeclaringType!.FullName == "System.Reflection.ObfuscationAttribute");
            var attr = obfAttributes.FirstOrDefault();
            if (attr == null)
                return null;

            foreach (var attr2 in obfAttributes.ToList())
                member.CustomAttributes.Remove(attr2);

            var exclude = attr.Signature!.NamedArguments
                .Any(a => a.MemberName == "Exclude" && a.Argument.Element!.Equals(true));
            var applyToMembers = attr.Signature!.NamedArguments
                .Any(a => a.MemberName == "ApplyToMembers" && a.Argument.Element!.Equals(true));
            return (exclude, applyToMembers);
        }

        private bool DoesPropagateExclusion(IMetadataMember member)
        {
            return _excluded.TryGetValue(member, out bool applyToMembers)
                && applyToMembers;
        }
    }
}
