using AsmResolver.DotNet;
using SimEi.Obfuscator.Renaming.Permission;
using SimEi.Obfuscator.Renaming.RenameLog;

namespace SimEi.Obfuscator.Renaming
{
    internal class Renamer : ModuleVisitorBase
    {
        private readonly INamingContext _namingContext;
        private readonly IRenamingPermissions _permissions;
        private readonly IRenamingLogger _logger;

        public Renamer(INamingContext namingContext, IRenamingPermissions permissions, IRenamingLogger logger)
        {
            _namingContext = namingContext;
            _permissions = permissions;
            _logger = logger;
        }


        public override void VisitType(TypeDefinition type)
        {
            if (!_permissions.CanRename(type))
                return;

            _logger.Track(type);

            type.Name = _namingContext.GetNextName(type.DeclaringType, RenamedElementType.Type);
            type.Namespace = null;

            foreach (var gparam in type.GenericParameters)
                gparam.Name = _namingContext.GetNextName(type, RenamedElementType.GenericParameter);
        }

        public override void VisitField(FieldDefinition field)
        {
            if (!_permissions.CanRename(field))
                return;

            _logger.Track(field);

            if (!field.IsSpecialName && !field.IsRuntimeSpecialName)
                field.Name = _namingContext.GetNextName(field.DeclaringType, RenamedElementType.Field);
        }

        public override void VisitProp(PropertyDefinition prop)
        {
            if (!_permissions.CanRename(prop))
                return;

            _logger.Track(prop);

            if (!prop.IsSpecialName && !prop.IsRuntimeSpecialName)
                prop.Name = _namingContext.GetNextName(prop.DeclaringType, RenamedElementType.Property);
        }

        public override void VisitEvent(EventDefinition evt)
        {
            if (!_permissions.CanRename(evt))
                return;

            _logger.Track(evt);

            if (!evt.IsSpecialName && !evt.IsRuntimeSpecialName)
                evt.Name = _namingContext.GetNextName(evt.DeclaringType, RenamedElementType.Event);
        }

        public override void VisitMethod(MethodDefinition method)
        {
            foreach (var param in method.ParameterDefinitions)
            {
                if (!_permissions.CanRename(param))
                    continue;

                param.Name = null;
            }

            if (!_permissions.CanRename(method))
                return;

            _logger.Track(method);

            if (!method.IsSpecialName && !method.IsRuntimeSpecialName)
                method.Name = _namingContext.GetNextName(method.DeclaringType, RenamedElementType.Method);

            foreach (var gparam in method.GenericParameters)
                gparam.Name = _namingContext.GetNextName(method.DeclaringType, RenamedElementType.GenericParameter);
        }
    }
}
