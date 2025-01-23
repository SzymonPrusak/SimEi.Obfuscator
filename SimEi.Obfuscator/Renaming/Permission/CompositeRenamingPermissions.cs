using AsmResolver.DotNet;

namespace SimEi.Obfuscator.Renaming.Permission
{
    internal class CompositeRenamingPermissions : IRenamingPermissions
    {
        private readonly IRenamingPermissions[] _permissions;

        public CompositeRenamingPermissions(params IRenamingPermissions[] permissions)
        {
            _permissions = permissions;
        }


        public bool CanRename(IMetadataMember member)
        {
            foreach (var p in _permissions)
            {
                if (!p.CanRename(member))
                    return false;
            }
            return true;
        }
    }
}
