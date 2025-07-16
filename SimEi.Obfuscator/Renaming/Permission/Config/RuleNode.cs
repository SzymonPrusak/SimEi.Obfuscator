using AsmResolver.Collections;
using AsmResolver.DotNet;
using SimEi.Obfuscator.Config;

namespace SimEi.Obfuscator.Renaming.Permission.Config
{
    internal class RuleNode
    {
        private readonly Rule _rule;

        public RuleNode(Rule rule, RuleNode? parent)
        {
            _rule = rule;

            Parent = parent;

            Children = rule.Rules?
                .Select(r => new RuleNode(r, this))
                .ToList()
                ?? Enumerable.Empty<RuleNode>();
        }



        public RuleNode? Parent { get; }
        public IEnumerable<RuleNode> Children { get; }



        public IEnumerable<RuleNode> GetAllSubRules()
        {
            return Children
                .SelectMany(c => c.GetAllSubRules())
                .Prepend(this);
        }


        public ActionType? ResolveAction(IMetadataMember member, IMetadataResolver resolver)
        {
            IMetadataMember? cur = member;
            var rule = this;
            while (cur != null && rule != null)
            {
                if (!rule.IsApplicable(cur, resolver))
                    return null;

                rule = rule.Parent;
                cur = GetParent(cur);
            }

            return rule == null ? _rule.Action : null;
        }


        private bool IsApplicable(IMetadataMember member, IMetadataResolver resolver)
        {
            if (_rule.Name != null)
            {
                string name = member switch
                {
                    IFullNameProvider fnp => fnp.Name!,
                    ParameterDefinition pd => pd.Name!,
                    _ => throw new ArgumentException()
                };
                if (name != _rule.Name)
                    return false;
            }
            if (_rule.FullName != null)
            {
                string fname = member switch
                {
                    TypeDefinition typeDef => typeDef.FullName,
                    IMemberDefinition memDef => $"{memDef.DeclaringType!.FullName}.{memDef.Name}",
                    ParameterDefinition pd => $"{pd.Method!.DeclaringType!.FullName}.{pd.Method!.Name}:{pd.Name}",
                    _ => throw new ArgumentException()
                };
                if (fname != _rule.FullName)
                    return false;
            }
            if (_rule.HasAttribute != null)
            {
                bool hasAttr = ((IHasCustomAttribute)member).CustomAttributes
                    .Any(a => a.Constructor!.DeclaringType!.FullName == _rule.HasAttribute);
                if (!hasAttr)
                    return false;
            }
            if (_rule.Public.HasValue)
            {
                bool isPublic = member switch
                {
                    TypeDefinition td => IsPublic(td),
                    FieldDefinition fd => (fd.IsPublic || (fd.IsFamily && !fd.IsAssembly)) && IsPublic(fd.DeclaringType!),
                    PropertyDefinition prd => prd.GetMethod != null && IsPublic(prd.GetMethod) || prd.SetMethod != null && IsPublic(prd.SetMethod),
                    EventDefinition ed => (ed.AddMethod != null && IsPublic(ed.AddMethod) || ed.RemoveMethod != null && IsPublic(ed.RemoveMethod)) && IsPublic(ed.DeclaringType!),
                    MethodDefinition md => IsPublic(md),
                    ParameterDefinition pd => IsPublic(pd.Method!),
                    _ => throw new ArgumentException()
                };
                if (isPublic ^ _rule.Public.Value)
                    return false;
            }
            if (_rule.SubtypeOf != null)
            {
                if (!(member is TypeDefinition typeDef))
                    return false;

                var st = typeDef.BaseType!;
                bool applicable = false;
                while (st != null)
                {
                    var stDef = resolver.ResolveType(st)!;
                    if (_rule.SubtypeOf == stDef.FullName!.ToString())
                    {
                        applicable = true;
                        break;
                    }
                    st = stDef.BaseType;
                }
                if (!applicable)
                    return false;
            }
            return true;
        }

        private IMetadataMember? GetParent(IMetadataMember member)
        {
            return member switch
            {
                IOwnedCollectionElement<TypeDefinition> td => td.Owner,
                ParameterDefinition pd => pd.Method,
                _ => throw new ArgumentException()
            };
        }

        private bool IsPublic(TypeDefinition typeDef)
        {
            return typeDef.IsPublic && typeDef.DeclaringType == null
                || (typeDef.IsNestedPublic || typeDef.IsNestedFamily && !typeDef.IsNestedAssembly) && IsPublic(typeDef.DeclaringType!);
        }

        private bool IsPublic(MethodDefinition method)
        {
            return (method.IsPublic || (method.IsFamily && !method.IsAssembly)) && IsPublic(method.DeclaringType!);
        }
    }
}
