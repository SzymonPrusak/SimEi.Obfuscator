using AsmResolver.DotNet;
using AsmResolver.DotNet.Signatures;

namespace SimEi.Obfuscator.Renaming.Reference.Resolving
{
    internal class ResolvedSignatureReference : IResolvedReference<TypeSignature>
    {
        private readonly TypeSignature _originalSignature;
        private readonly TypeDefinition _resolvedType;
        private readonly IEnumerable<IResolvedReference<TypeSignature>>? _genericArgs;

        public ResolvedSignatureReference(TypeSignature originalSignature, TypeDefinition resolvedType,
            IEnumerable<IResolvedReference<TypeSignature>>? genericArgs = null)
        {
            _originalSignature = originalSignature;
            _resolvedType = resolvedType;
            _genericArgs = genericArgs;
        }


        public TypeSignature GetResolved()
        {
            string name = _resolvedType.Name;
            var imported = _originalSignature.Module!.DefaultImporter.ImportType(_resolvedType);
            if (_originalSignature is SzArrayTypeSignature)
                return imported.MakeSzArrayType();
            else if (_originalSignature is ArrayTypeSignature at)
                return imported.MakeArrayType([..at.Dimensions]);
            else if (_originalSignature is ByReferenceTypeSignature)
                return imported.MakeByReferenceType();
            else if (_originalSignature is GenericInstanceTypeSignature git)
            {
                var gargs = _genericArgs!
                    .Select(p => p.GetResolved())
                    .ToArray();
                return imported.MakeGenericInstanceType(gargs);
            }
            else
                return imported.ToTypeSignature();
        }
    }
}
