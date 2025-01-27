using AsmResolver.DotNet;

namespace SimEi.Obfuscator.Renaming.Permission
{
    internal class RuntimeExcludedPermissions : IRenamingPermissions
    {
        public bool CanRename(IMetadataMember member)
        {
            if (member is TypeDefinition type)
            {
                return !type.IsModuleType
                    && type.Namespace != "Microsoft.CodeAnalysis"
                    && type.Namespace != "System.Runtime.CompilerServices";
            }
            if (member is MethodDefinition method)
            {
                return !method.DeclaringType!.BaseType?.Name!.Equals("MulticastDelegate") ?? true;
            }
            return true;
        }
    }
}
