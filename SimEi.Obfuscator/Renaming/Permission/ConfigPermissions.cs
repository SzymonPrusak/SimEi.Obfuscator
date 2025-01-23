using AsmResolver.DotNet;

namespace SimEi.Obfuscator.Renaming.Permission
{
    internal class ConfigPermissions : IRenamingPermissions
    {
        public bool CanRename(IMetadataMember member)
        {
            return true;
        }
    }
}
