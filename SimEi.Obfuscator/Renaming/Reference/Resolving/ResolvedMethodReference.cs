using AsmResolver.DotNet;
using AsmResolver.DotNet.Signatures;

namespace SimEi.Obfuscator.Renaming.Reference.Resolving
{
    internal class ResolvedMethodReference : ResolvedReferenceBase<IMethodDefOrRef>
    {
        private readonly IMethodDefOrRef _original;
        private readonly IMethodDefOrRef _resolved;

        private readonly IEnumerable<IResolvedReference<TypeSignature>>? _declaringTypeGenericArgs;

        public ResolvedMethodReference(IMethodDefOrRef original, IMethodDefOrRef resolved,
            IEnumerable<IResolvedReference<TypeSignature>>? declaringTypeGenericArgs)
        {
            _original = original;
            _resolved = resolved;

            _declaringTypeGenericArgs = declaringTypeGenericArgs;
        }


        protected override IMethodDefOrRef Resolve()
        {
            var resolved = _resolved;
            if (_original.DeclaringType is TypeSpecification typeSpec)
            {
                var args = _declaringTypeGenericArgs!
                    .Select(a => a.GetResolved())
                    .ToArray();
                var genericInstance = _resolved.DeclaringType!.MakeGenericInstanceType(args);
                var resolvedTs = new TypeSpecification(genericInstance);
                resolved = new MemberReference(resolvedTs, _resolved.Name, _resolved.Signature);
            }
            return _original.Module!.DefaultImporter.ImportMethod(resolved);
        }
    }
}
