using AsmResolver.DotNet;
using AsmResolver.DotNet.Signatures;

namespace SimEi.Obfuscator.Renaming.Reference.Resolving
{
    internal class ResolvedMethodReference : ResolvedReferenceBase<IMethodDescriptor>
    {
        private readonly IMethodDescriptor _original;
        private readonly MethodDefinition _resolved;

        private readonly IEnumerable<IResolvedReference<TypeSignature>>? _declaringTypeGenericArgs;
        private readonly IEnumerable<IResolvedReference<TypeSignature>>? _methodGenericArgs;

        public ResolvedMethodReference(IMethodDescriptor original, MethodDefinition resolved,
            IEnumerable<IResolvedReference<TypeSignature>>? declaringTypeGenericArgs,
            IEnumerable<IResolvedReference<TypeSignature>>? methodGenericArgs)
        {
            _original = original;
            _resolved = resolved;

            _declaringTypeGenericArgs = declaringTypeGenericArgs;
            _methodGenericArgs = methodGenericArgs;
        }


        protected override IMethodDescriptor Resolve()
        {
            IMethodDefOrRef? resolved = _resolved;
            if (_original.DeclaringType is TypeSpecification typeSpec)
            {
                var args = _declaringTypeGenericArgs!
                    .Select(a => a.GetResolved())
                    .ToArray();
                var genericInstance = _resolved.DeclaringType!.MakeGenericInstanceType(args);
                var targetType = new TypeSpecification(genericInstance);
                resolved = new MemberReference(targetType, _original.Name, _resolved.Signature);
            }

            if (_original is MethodSpecification mSpec)
            {
                var args = _methodGenericArgs!
                    .Select(a => a.GetResolved())
                    .ToArray();
                var genericInstance = resolved.MakeGenericInstanceMethod(args);
                return _original.Module!.DefaultImporter.ImportMethod(genericInstance);
            }
            return _original.Module!.DefaultImporter.ImportMethod(resolved);
        }
    }
}
