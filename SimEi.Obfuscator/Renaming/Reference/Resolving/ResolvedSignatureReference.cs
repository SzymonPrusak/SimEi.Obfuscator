using AsmResolver.DotNet;
using AsmResolver.DotNet.Signatures;

namespace SimEi.Obfuscator.Renaming.Reference.Resolving
{
    internal class ResolvedSignatureReference : ResolvedReferenceBase<TypeSignature>
    {
        private readonly TypeSignature _originalSignature;
        private readonly TypeDefinition _resolvedType;
        private readonly IEnumerable<IResolvedReference<TypeSignature>>? _genericArgs;

        public ResolvedSignatureReference(TypeSignature originalSignature, TypeDefinition resolvedType,
            IEnumerable<IResolvedReference<TypeSignature>>? genericArgs)
        {
            _originalSignature = originalSignature;
            _resolvedType = resolvedType;
            _genericArgs = genericArgs;
        }


        protected override TypeSignature Resolve() => ResolveTraversing(_originalSignature);

        private TypeSignature ResolveTraversing(TypeSignature original)
        {
            switch (original)
            {
                case GenericInstanceTypeSignature gitSig:
                    var imported = original.Module!.DefaultImporter.ImportType(_resolvedType);
                    var gargs = _genericArgs!
                        .Select(p => p.GetResolved())
                        .ToArray();
                    return imported.MakeGenericInstanceType(gargs);

                case TypeDefOrRefSignature:
                case CorLibTypeSignature:
                case GenericParameterSignature:
                    return original.Module!.DefaultImporter.ImportType(_resolvedType).ToTypeSignature();

                // TODO
                //case FunctionPointerTypeSignature fpSig:
                //case SentinelParameterTypeSignature:

                case SzArrayTypeSignature saSig:
                    return ResolveTraversing(saSig.BaseType).MakeSzArrayType();

                case ArrayTypeSignature aSig:
                    return ResolveTraversing(aSig.BaseType).MakeArrayType([.. aSig.Dimensions]);

                case BoxedTypeSignature bSig:
                    return new BoxedTypeSignature(ResolveTraversing(bSig.BaseType));

                case ByReferenceTypeSignature brSig:
                    return ResolveTraversing(brSig.BaseType).MakeByReferenceType();

                case CustomModifierTypeSignature cmSig:
                    return ResolveTraversing(cmSig.BaseType).MakeModifierType(cmSig.ModifierType, cmSig.IsRequired);

                case PinnedTypeSignature pinSig:
                    return ResolveTraversing(pinSig.BaseType).MakePinnedType();

                case PointerTypeSignature pointSig:
                    return ResolveTraversing(pointSig.BaseType).MakePointerType();
            }

            throw new ArgumentException();
        }
    }
}
