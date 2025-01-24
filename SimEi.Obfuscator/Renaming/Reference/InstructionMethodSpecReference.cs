using AsmResolver.DotNet;
using AsmResolver.DotNet.Signatures;
using AsmResolver.PE.DotNet.Cil;
using SimEi.Obfuscator.Renaming.Reference.Resolving;

namespace SimEi.Obfuscator.Renaming.Reference
{
    internal class InstructionMethodSpecReference : ITrackedReference
    {
        private readonly CilInstruction _instruction;

        private readonly IResolvedReference<IMethodDefOrRef> _methodRef;
        private readonly IEnumerable<IResolvedReference<TypeSignature>> _genericArgs;

        public InstructionMethodSpecReference(CilInstruction instruction, ReferenceResolver resolver)
        {
            _instruction = instruction;

            var spec = (MethodSpecification)instruction.Operand!;
            _methodRef = resolver.Resolve(spec.Method!);
            _genericArgs = spec.Signature!.TypeArguments
                .Select(resolver.ResolveSig)
                .ToList();
        }


        public void Fix()
        {
            var args = _genericArgs
                .Select(a => a.GetResolved())
                .ToArray();
            _instruction.Operand = new MethodSpecification(_methodRef.GetResolved(), new GenericInstanceMethodSignature(args));
        }
    }
}
