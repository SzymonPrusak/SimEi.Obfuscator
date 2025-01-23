using AsmResolver.DotNet;

namespace SimEi.Obfuscator.Renaming.Permission
{
    internal interface IRenamingPermissions
    {
        bool CanRename(IMetadataMember member);
    }
}
