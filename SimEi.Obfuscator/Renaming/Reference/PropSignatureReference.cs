using AsmResolver.DotNet.Signatures;
using SimEi.Obfuscator.Renaming.Reference.Resolving;

namespace SimEi.Obfuscator.Renaming.Reference
{
    internal class PropSignatureReference : ITrackedReference
    {
        private readonly PropertySignature _signature;

        private readonly IResolvedReference<TypeSignature> _retTypeRef;
        private readonly List<IResolvedReference<TypeSignature>> _paramTypeRefs;

        public PropSignatureReference(PropertySignature signature)
        {
            _signature = signature;

            _retTypeRef = ReferenceResolver.ResolveSig(signature.ReturnType);
            _paramTypeRefs = signature.ParameterTypes
                .Select(ReferenceResolver.ResolveSig)
                .ToList();
        }


        public void Fix()
        {
            _signature.ReturnType = _retTypeRef.GetResolved();
            foreach ((int index, var sigRef) in Enumerable.Range(0, _signature.ParameterTypes.Count).Zip(_paramTypeRefs))
                _signature.ParameterTypes[index] = sigRef.GetResolved();
        }
    }
}
