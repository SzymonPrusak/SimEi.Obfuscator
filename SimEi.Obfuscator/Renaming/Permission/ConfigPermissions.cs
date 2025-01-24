using AsmResolver.DotNet;
using SimEi.Obfuscator.Config;

namespace SimEi.Obfuscator.Renaming.Permission
{
    internal class ConfigPermissions : IRenamingPermissions
    {
        private readonly ConfigDocument _config;
        private readonly IMetadataResolver _metadataResolver;

        public ConfigPermissions(ConfigDocument config, IMetadataResolver metadataResolver)
        {
            _config = config;
            _metadataResolver = metadataResolver;
        }



        public bool CanRename(IMetadataMember member)
        {
            return member switch
            {
                TypeDefinition type => CheckType(type),
                FieldDefinition field => CheckField(field),
                PropertyDefinition prop => CheckProp(prop),
                EventDefinition evt => CheckEvent(evt),
                MethodDefinition method => CheckMethod(method),
                ParameterDefinition param => CheckParam(param),
                _ => throw new ArgumentException()
            };
        }


        private bool CheckType(TypeDefinition type)
        {
            var rules = GetApplicableRules(type.Module!);
            foreach (var rule in rules)
            {
                if (IsApplicable(rule, type))
                    return false;
            }
            return true;
        }


        private bool CheckField(FieldDefinition field)
        {
            return CheckMember(field.DeclaringType!, field.Module!, field.Name!, field.IsPrivate, RenamedElementType.Field);
        }


        private bool CheckProp(PropertyDefinition prop)
        {
            return CheckMember(prop.DeclaringType!, prop.Module!, prop.Name!, false, RenamedElementType.Property);
        }


        private bool CheckEvent(EventDefinition evt)
        {
            return CheckMember(evt.DeclaringType!, evt.Module!, evt.Name!, false, RenamedElementType.Event);
        }


        private bool CheckMethod(MethodDefinition method)
        {
            return CheckMember(method.DeclaringType!, method.Module!, method.Name!, method.IsPrivate, RenamedElementType.Event);
        }


        private bool CheckParam(ParameterDefinition param)
        {
            return CheckMethod(param.Method!);
        }


        private bool CheckMember(TypeDefinition type, ModuleDefinition module, string name, bool isPrivate, RenamedElementType elType)
        {
            var rules = GetApplicableRules(type.Module!);
            foreach (var rule in rules)
            {
                if (!IsApplicable(rule, type))
                    continue;

                if (rule.ExplicitMembers != null)
                {
                    foreach (var memberRule in rule.ExplicitMembers)
                    {
                        if (IsApplicable(memberRule, name, isPrivate, elType))
                            return false;
                    }
                }

                if (!(rule?.SkipMembers ?? false))
                    return false;
            }
            return true;
        }

        private IEnumerable<ExcludeClass> GetApplicableRules(ModuleDefinition module)
        {
            var moduleConfig = _config.Assemblies
                .FirstOrDefault(a => a.Path == module.Assembly!.FullName);
            return moduleConfig != null
                ? _config.GlobalExcludes.Concat(moduleConfig.Excludes)
                : _config.GlobalExcludes;
        }

        private bool IsApplicable(ExcludeClass rule, TypeDefinition type)
        {
            if (rule.Name != null && rule.Name != type.Name!.ToString())
                return false;
            if (rule.Public.HasValue && (type.IsNotPublic || type.IsNestedPrivate) ^ !rule.Public.Value)
                return false;
            if (rule.SubtypeOf != null)
            {
                var st = type.BaseType!;
                bool applicable = false;
                while (st != null)
                {
                    var stDef = _metadataResolver.ResolveType(st)!;
                    if (rule.SubtypeOf == stDef.Name!.ToString())
                    {
                        applicable = true;
                        break;
                    }
                }
                if (!applicable)
                    return false;
            }
            return true;
        }

        private bool IsApplicable(MemberInfo rule, string name, bool isPrivate, RenamedElementType type)
        {
            if (rule.Name != null && rule.Name != name)
                return false;
            if (rule.Private.HasValue && (rule.Private.Value ^ isPrivate))
                return false;
            if (rule.ElementType.HasValue && rule.ElementType.Value != type)
                return false;
            return true;
        }
    }
}
