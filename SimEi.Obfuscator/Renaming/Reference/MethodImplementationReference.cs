using AsmResolver.DotNet;
using SimEi.Obfuscator.Renaming.Reference.Resolving;

namespace SimEi.Obfuscator.Renaming.Reference
{
    internal class MethodImplementationReference : ITrackedReference
    {
        private readonly TypeDefinition _type;
        private readonly int _index;

        private readonly IResolvedReference<IMethodDefOrRef> _resolved;

        public MethodImplementationReference(TypeDefinition type, int index, ReferenceResolver resolver)
        {
            _type = type;
            _index = index;

            _resolved = resolver.Resolve(type.MethodImplementations[index].Declaration!);
        }


        public void Fix()
        {
            var impl = _type.MethodImplementations[_index];
            var resolved = _resolved.GetResolved();
            _type.MethodImplementations[_index] = new MethodImplementation(resolved, impl.Body);
        }
    }
}
