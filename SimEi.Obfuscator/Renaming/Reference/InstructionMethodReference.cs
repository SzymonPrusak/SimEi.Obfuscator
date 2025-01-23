using AsmResolver.DotNet;
using AsmResolver.PE.DotNet.Cil;
using SimEi.Obfuscator.Renaming.Reference.Resolving;

namespace SimEi.Obfuscator.Renaming.Reference
{
    internal class InstructionMethodReference : ITrackedReference
    {
        private readonly CilInstruction _instruction;

        private readonly IResolvedReference<IMethodDefOrRef> _methodRef;

        public InstructionMethodReference(CilInstruction instruction)
        {
            _instruction = instruction;

            var mr = (MemberReference)instruction.Operand!;
            _methodRef = ReferenceResolver.Resolve((IMethodDefOrRef)mr);
        }


        public void Fix()
        {
            _instruction.Operand = _methodRef.GetResolved();
        }
    }
}
