using AsmResolver.DotNet;
using AsmResolver.DotNet.Signatures;

namespace SimEi.Obfuscator.Renaming.Reference.Resolving
{
    internal class ResolvedTypeReference : ResolvedReferenceBase<ITypeDefOrRef>
    {
        private readonly ITypeDefOrRef _original;
        private readonly TypeDefinition _resolved;

        private readonly IEnumerable<IResolvedReference<TypeSignature>>? _genericArgs;

        public ResolvedTypeReference(ITypeDefOrRef original, TypeDefinition resolved,
            IEnumerable<IResolvedReference<TypeSignature>>? genericArgs = null)
        {
            _original = original;
            _resolved = resolved;

            _genericArgs = genericArgs;
        }


        protected override ITypeDefOrRef Resolve()
        {
            if (_genericArgs != null)
            {
                var args = _genericArgs
                    .Select(a => a.GetResolved())
                    .ToArray();
                var genericInstance = _resolved.MakeGenericInstanceType(args);
                return new TypeSpecification(_original.Module!.DefaultImporter.ImportTypeSignature(genericInstance));
            }
            return _original.Module!.DefaultImporter.ImportType(_resolved);
        }
    }
}
