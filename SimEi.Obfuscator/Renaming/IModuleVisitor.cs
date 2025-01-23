using AsmResolver.DotNet;
using AsmResolver.DotNet.Code.Cil;
using AsmResolver.PE.DotNet.Cil;

namespace SimEi.Obfuscator.Renaming
{
    internal interface IModuleVisitor
    {
        void VisitType(TypeDefinition type, IReadOnlyList<TypeDefinition> declaringTypes);
        void VisitField(FieldDefinition field, IReadOnlyList<TypeDefinition> declaringTypes);
        void VisitProp(PropertyDefinition prop, IReadOnlyList<TypeDefinition> declaringTypes);
        void VisitEvent(EventDefinition evt, IReadOnlyList<TypeDefinition> declaringTypes);
        void VisitMethod(MethodDefinition method, IReadOnlyList<TypeDefinition> declaringTypes);

        void VisitLocal(CilLocalVariable local);
        void VisitInstruction(CilInstruction instruction);
        void VisitExceptionHandler(CilExceptionHandler excHandler);
    }
}
