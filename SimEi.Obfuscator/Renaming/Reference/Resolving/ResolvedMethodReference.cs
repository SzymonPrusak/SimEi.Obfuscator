using AsmResolver.DotNet;
using AsmResolver.DotNet.Signatures;

namespace SimEi.Obfuscator.Renaming.Reference.Resolving
{
    internal class ResolvedMethodReference : ResolvedReferenceBase<IMethodDefOrRef>
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


        protected override IMethodDefOrRef Resolve()
        {
            GenericContext genCtx = default;
            ITypeDefOrRef? targetType = null;
            if (_original.DeclaringType is TypeSpecification typeSpec)
            {
                var args = _declaringTypeGenericArgs!
                    .Select(a => a.GetResolved())
                    .ToArray();
                var genericInstance = _resolved.DeclaringType!.MakeGenericInstanceType(args);
                targetType = new TypeSpecification(genericInstance);
                genCtx = new GenericContext(genericInstance, null);
            }
            else
                targetType = _resolved.DeclaringType;

            if (_original is MethodSpecification mSpec)
            {
                var args = _methodGenericArgs!
                    .Select(a => a.GetResolved())
                    .ToArray();
                var genericInstance = _resolved.MakeGenericInstanceMethod(args);
                genCtx = new GenericContext(genCtx.Type, genericInstance.Signature);
            }

            var finalSig = _resolved.Signature!.InstantiateGenericTypes(genCtx);
            var targetRef = new MemberReference(targetType, _original.Name, finalSig);
            return _original.Module!.DefaultImporter.ImportMethod(_resolved);
        }
    }
}
