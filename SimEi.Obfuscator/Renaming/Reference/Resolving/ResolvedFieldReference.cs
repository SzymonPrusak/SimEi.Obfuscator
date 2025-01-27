using AsmResolver.DotNet;
using AsmResolver.DotNet.Signatures;

namespace SimEi.Obfuscator.Renaming.Reference.Resolving
{
    internal class ResolvedFieldReference : ResolvedReferenceBase<IFieldDescriptor>
    {
        private readonly IFieldDescriptor _original;
        private readonly FieldDefinition _resolved;

        private readonly IEnumerable<IResolvedReference<TypeSignature>>? _declaringTypeGenericArgs;

        public ResolvedFieldReference(IFieldDescriptor original, FieldDefinition resolved,
            IEnumerable<IResolvedReference<TypeSignature>>? declaringTypeGenericArgs)
        {
            _original = original;
            _resolved = resolved;

            _declaringTypeGenericArgs = declaringTypeGenericArgs;
        }


        protected override IFieldDescriptor Resolve()
        {
            IFieldDescriptor resolved = _resolved;
            if (_original.DeclaringType is TypeSpecification typeSpec)
            {
                var args = _declaringTypeGenericArgs!
                    .Select(a => a.GetResolved())
                    .ToArray();
                var genericInstance = _resolved.DeclaringType!.MakeGenericInstanceType(args.ToArray());
                var resolvedTs = new TypeSpecification(genericInstance);
                resolved = new MemberReference(resolvedTs, _resolved.Name, _resolved.Signature);
            }
            return _original.Module!.DefaultImporter.ImportField(resolved);
        }
    }
}
