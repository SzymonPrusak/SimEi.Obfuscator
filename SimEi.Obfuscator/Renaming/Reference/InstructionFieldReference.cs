using AsmResolver.DotNet;
using AsmResolver.PE.DotNet.Cil;
using SimEi.Obfuscator.Renaming.Reference.Resolving;

namespace SimEi.Obfuscator.Renaming.Reference
{
    internal class InstructionFieldReference : ITrackedReference
    {
        private readonly CilInstruction _instruction;

        private readonly IResolvedReference<IFieldDescriptor> _fieldRef;

        public InstructionFieldReference(CilInstruction instruction, ReferenceResolver resolver)
        {
            _instruction = instruction;

            var mr = (MemberReference)instruction.Operand!;
            _fieldRef = resolver.Resolve((IFieldDescriptor)mr);
        }


        public void Fix()
        {
            _instruction.Operand = _fieldRef.GetResolved();
        }
    }
}
