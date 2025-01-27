using AsmResolver.DotNet;
using AsmResolver.PE.DotNet.Cil;
using SimEi.Obfuscator.Renaming.Reference.Resolving;

namespace SimEi.Obfuscator.Renaming.Reference
{
    internal class InstructionMethodReference : ITrackedReference
    {
        private readonly CilInstruction _instruction;

        private readonly IResolvedReference<IMethodDescriptor> _methodRef;

        public InstructionMethodReference(CilInstruction instruction, ReferenceResolver resolver)
        {
            _instruction = instruction;

            _methodRef = resolver.Resolve((IMethodDescriptor)instruction.Operand!);
        }


        public void Fix()
        {
            _instruction.Operand = _methodRef.GetResolved();
        }
    }
}
