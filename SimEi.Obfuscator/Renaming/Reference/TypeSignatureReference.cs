using AsmResolver.DotNet;
using SimEi.Obfuscator.Renaming.Reference.Resolving;

namespace SimEi.Obfuscator.Renaming.Reference
{
    internal class TypeSignatureReference : ITrackedReference
    {
        private readonly TypeDefinition _type;

        private readonly IResolvedReference<ITypeDefOrRef>? _baseTypeRef;
        private readonly IEnumerable<IResolvedReference<ITypeDefOrRef>> _interfaceRefs;

        public TypeSignatureReference(TypeDefinition type)
        {
            _type = type;

            _baseTypeRef = type.BaseType != null 
                ? ReferenceResolver.Resolve(type.BaseType!)
                : null;
            _interfaceRefs = type.Interfaces
                .Select(i => ReferenceResolver.Resolve(i.Interface!))
                .ToList();
        }


        public void Fix()
        {
            _type.BaseType = _baseTypeRef?.GetResolved();
            foreach ((var iface, var r) in _type.Interfaces.Zip(_interfaceRefs))
                iface.Interface = r.GetResolved();
        }
    }
}
