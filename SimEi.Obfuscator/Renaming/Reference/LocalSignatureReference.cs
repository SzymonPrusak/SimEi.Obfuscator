using AsmResolver.DotNet.Code.Cil;
using AsmResolver.DotNet.Signatures;
using SimEi.Obfuscator.Renaming.Reference.Resolving;

namespace SimEi.Obfuscator.Renaming.Reference
{
    internal class LocalSignatureReference : ITrackedReference
    {
        private readonly CilLocalVariable _local;

        private readonly IResolvedReference<TypeSignature> _typeRef;

        public LocalSignatureReference(CilLocalVariable local, ReferenceResolver resolver)
        {
            _local = local;

            _typeRef = resolver.ResolveSig(local.VariableType);
        }


        public void Fix()
        {
            _local.VariableType = _typeRef.GetResolved();
        }
    }
}
