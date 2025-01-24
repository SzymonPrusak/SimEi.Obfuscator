using AsmResolver.DotNet;
using AsmResolver.PE.DotNet.Cil;
using SimEi.Obfuscator.Renaming.Reference.Resolving;

namespace SimEi.Obfuscator.Renaming.Reference
{
    internal class InstructionTypeReference : ITrackedReference
    {
        private readonly CilInstruction _instruction;

        private readonly IResolvedReference<ITypeDefOrRef> _resolved;

        public InstructionTypeReference(CilInstruction instruction, ReferenceResolver resolver)
        {
            _instruction = instruction;

            _resolved = resolver.Resolve((ITypeDefOrRef)instruction.Operand!);
        }


        public void Fix()
        {
            _instruction.Operand = _resolved.GetResolved();
        }
    }
}
