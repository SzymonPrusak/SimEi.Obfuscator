using AsmResolver.DotNet;

namespace SimEi.Obfuscator.Renaming.Permission
{
    internal class ConcreteExcludedPermissions : IRenamingPermissions
    {
        private readonly HashSet<IMetadataMember> _members;

        public ConcreteExcludedPermissions(IEnumerable<IMetadataMember> members)
        {
            _members = members.ToHashSet();
        }


        public bool CanRename(IMetadataMember member)
        {
            return !_members.Contains(member);
        }
    }
}
