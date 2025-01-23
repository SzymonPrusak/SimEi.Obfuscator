using AsmResolver.DotNet;

namespace SimEi.Obfuscator.Renaming.SigGraph
{
    internal class SigComponent
    {
        private readonly MethodNode _master;

        public SigComponent(MethodNode master)
        {
            _master = master;
        }


        public IEnumerable<MethodDefinition> Nodes => _master.ConnectedNodes.Select(n => n.Method);


        public bool AreAllMethodsControlled(IMetadataResolver resolver, IReadOnlyCollection<ModuleDefinition> controlledModules)
        {
            return Nodes.All(n => controlledModules.Contains(resolver.ResolveType(n.DeclaringType)!.Module!));
        }
    }
}
