using AsmResolver.DotNet;
using SimEi.Obfuscator.Renaming.Permission;

namespace SimEi.Obfuscator.Renaming
{
    internal class Renamer : ModuleVisitorBase
    {
        private readonly INamingContext _namingContext;
        private readonly IRenamingPermissions _permissions;

        public Renamer(INamingContext namingContext, IRenamingPermissions permissions)
        {
            _namingContext = namingContext;
            _permissions = permissions;
        }


        public override void VisitType(TypeDefinition type, IReadOnlyList<TypeDefinition> declaringTypes)
        {
            if (type.IsModuleType || !_permissions.CanRename(type))
                return;

            type.Name = _namingContext.GetNextName(type.DeclaringType, RenamedElementType.Type);
            type.Namespace = null;

            foreach (var gparam in type.GenericParameters)
                gparam.Name = _namingContext.GetNextName(type, RenamedElementType.GenericParameter);
        }

        public override void VisitField(FieldDefinition field, IReadOnlyList<TypeDefinition> declaringTypes)
        {
            if (!_permissions.CanRename(field))
                return;

            field.Name = _namingContext.GetNextName(field.DeclaringType, RenamedElementType.Field);
        }

        public override void VisitProp(PropertyDefinition prop, IReadOnlyList<TypeDefinition> declaringTypes)
        {
            if (!_permissions.CanRename(prop))
                return;

            prop.Name = _namingContext.GetNextName(prop.DeclaringType, RenamedElementType.Property);
        }

        public override void VisitEvent(EventDefinition evt, IReadOnlyList<TypeDefinition> declaringTypes)
        {
            if (!_permissions.CanRename(evt))
                return;

            evt.Name = _namingContext.GetNextName(evt.DeclaringType, RenamedElementType.Event);
        }

        public override void VisitMethod(MethodDefinition method, IReadOnlyList<TypeDefinition> declaringTypes)
        {
            foreach (var param in method.ParameterDefinitions)
            {
                if (!_permissions.CanRename(param))
                    continue;

                param.Name = null;
            }

            if (!_permissions.CanRename(method))
                return;

            if (!method.IsConstructor)
                method.Name = _namingContext.GetNextName(method.DeclaringType, RenamedElementType.Method);

            foreach (var gparam in method.GenericParameters)
                gparam.Name = _namingContext.GetNextName(method.DeclaringType, RenamedElementType.GenericParameter);
        }
    }
}
