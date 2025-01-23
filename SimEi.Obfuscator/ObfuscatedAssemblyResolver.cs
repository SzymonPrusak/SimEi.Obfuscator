using AsmResolver.DotNet;
using AsmResolver;

namespace SimEi.Obfuscator
{
    internal class ObfuscatedAssemblyResolver : DotNetFrameworkAssemblyResolver
    {
        private readonly Dictionary<Utf8String, AssemblyDefinition> _defs;
        private readonly HashSet<ModuleDefinition> _modules;

        public ObfuscatedAssemblyResolver(params IEnumerable<AssemblyDefinition> defs)
        {
            _defs = defs.ToDictionary(d => d.Name!);
            _modules = defs
                .SelectMany(d => d.Modules)
                .ToHashSet();
        }


        public bool IsObfuscatedModule(ModuleDefinition module)
        {
            return _modules.Contains(module);
        }

        protected override AssemblyDefinition? ResolveImpl(AssemblyDescriptor assembly)
        {
            if (assembly.Name != (Utf8String?)null && _defs.TryGetValue(assembly.Name, out var a))
                return a;
            return base.ResolveImpl(assembly);
        }
    }
}
