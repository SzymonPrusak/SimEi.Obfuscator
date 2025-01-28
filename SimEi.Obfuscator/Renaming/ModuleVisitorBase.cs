using AsmResolver.DotNet;
using AsmResolver.DotNet.Code.Cil;
using AsmResolver.PE.DotNet.Cil;

namespace SimEi.Obfuscator.Renaming
{
    internal class ModuleVisitorBase : IModuleVisitor
    {
        public virtual void VisitType(TypeDefinition type) { }
        public virtual void VisitField(FieldDefinition field) { }
        public virtual void VisitProp(PropertyDefinition prop) { }
        public virtual void VisitEvent(EventDefinition evt) { }
        public virtual void VisitMethod(MethodDefinition method) { }

        public virtual void VisitLocal(CilLocalVariable local) { }
        public virtual void VisitInstruction(CilInstruction instruction) { }
        public virtual void VisitExceptionHandler(CilExceptionHandler excHandler) { }
    }
}
