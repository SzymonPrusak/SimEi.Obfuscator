using AsmResolver.DotNet;
using AsmResolver.DotNet.Code.Cil;
using AsmResolver.PE.DotNet.Cil;

namespace SimEi.Obfuscator.Renaming
{
    internal interface IModuleVisitor
    {
        void VisitType(TypeDefinition type);
        void VisitField(FieldDefinition field);
        void VisitProp(PropertyDefinition prop);
        void VisitEvent(EventDefinition evt);
        void VisitMethod(MethodDefinition method);

        void VisitLocal(CilLocalVariable local);
        void VisitInstruction(CilInstruction instruction);
        void VisitExceptionHandler(CilExceptionHandler excHandler);
    }
}
