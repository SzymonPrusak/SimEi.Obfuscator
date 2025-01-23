﻿using AsmResolver.DotNet;
using AsmResolver.DotNet.Code.Cil;
using AsmResolver.PE.DotNet.Cil;

namespace SimEi.Obfuscator.Renaming
{
    internal class ModuleVisitorBase : IModuleVisitor
    {
        public virtual void VisitType(TypeDefinition type, IReadOnlyList<TypeDefinition> declaringTypes) { }
        public virtual void VisitField(FieldDefinition field, IReadOnlyList<TypeDefinition> declaringTypes) { }
        public virtual void VisitProp(PropertyDefinition prop, IReadOnlyList<TypeDefinition> declaringTypes) { }
        public virtual void VisitEvent(EventDefinition evt, IReadOnlyList<TypeDefinition> declaringTypes) { }
        public virtual void VisitMethod(MethodDefinition method, IReadOnlyList<TypeDefinition> declaringTypes) { }

        public virtual void VisitLocal(CilLocalVariable local) { }
        public virtual void VisitInstruction(CilInstruction instruction) { }
        public virtual void VisitExceptionHandler(CilExceptionHandler excHandler) { }
    }
}
