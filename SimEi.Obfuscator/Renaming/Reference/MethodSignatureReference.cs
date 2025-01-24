using AsmResolver.DotNet;
using AsmResolver.DotNet.Signatures;
using SimEi.Obfuscator.Renaming.Reference.Resolving;

namespace SimEi.Obfuscator.Renaming.Reference
{
    internal class MethodSignatureReference : ITrackedReference
    {
        private readonly MethodDefinition _method;

        private readonly IResolvedReference<TypeSignature> _retTypeRef;
        private readonly List<IResolvedReference<TypeSignature>> _paramTypeRefs;

        public MethodSignatureReference(MethodDefinition method, ReferenceResolver resolver)
        {
            _method = method;

            _retTypeRef = resolver.ResolveSig(_method.Signature!.ReturnType);
            _paramTypeRefs = method.Parameters
                .Select(p => resolver.ResolveSig(p.ParameterType))
                .ToList();
        }


        public void Fix()
        {
            _method.Signature!.ReturnType = _retTypeRef.GetResolved();
            foreach((var param, var targetSig) in _method.Parameters.Zip(_paramTypeRefs))
                param.ParameterType = targetSig.GetResolved();
        }
    }
}
