using AsmResolver.DotNet;
using AsmResolver.DotNet.Code.Cil;
using AsmResolver.PE.DotNet.Cil;

namespace SimEi.Obfuscator.Renaming
{
    internal class ModuleVisitorBase : IModuleVisitor
    {
        public static void Visit(ModuleDefinition module, IModuleVisitor visitor)
        {
            foreach (var type in module.TopLevelTypes)
                Visit(type, visitor);
        }

        private static void Visit(TypeDefinition type, IModuleVisitor visitor)
        {
            visitor.VisitType(type);

            foreach (var field in type.Fields)
                visitor.VisitField(field);
            foreach (var prop in type.Properties)
                visitor.VisitProp(prop);
            foreach (var evt in type.Events)
                visitor.VisitEvent(evt);

            foreach (var method in type.Methods)
            {
                visitor.VisitMethod(method);

                if (method.CilMethodBody == null)
                    continue;

                foreach (var local in method.CilMethodBody.LocalVariables)
                    visitor.VisitLocal(local);

                foreach (var inst in method.CilMethodBody.Instructions)
                    visitor.VisitInstruction(inst);
                foreach (var excHandler in method.CilMethodBody.ExceptionHandlers)
                    visitor.VisitExceptionHandler(excHandler);
            }

            foreach (var st in type.NestedTypes)
                Visit(st, visitor);
        }


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
