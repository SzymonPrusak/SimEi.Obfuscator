using AsmResolver.DotNet;
using AsmResolver.DotNet.Signatures;
using SimEi.Obfuscator.Renaming.Reference.Resolving;

namespace SimEi.Obfuscator.Renaming.Reference
{
    internal class CustomAttributeReference : ITrackedReference
    {
        private readonly CustomAttribute _attribute;

        private readonly IResolvedReference<IMethodDefOrRef> _resolved;
        private readonly IEnumerable<IEnumerable<IResolvedReference<TypeSignature>?>> _fixedTypeArgs;
        private readonly IEnumerable<IEnumerable<IResolvedReference<TypeSignature>?>> _namedTypeArgs;

        public CustomAttributeReference(CustomAttribute attribute, ReferenceResolver resolver)
        {
            _attribute = attribute;
            
            _resolved = resolver.Resolve(attribute.Constructor!);
            _fixedTypeArgs = GetTypeArgs(attribute.Signature!.FixedArguments, resolver);
            _namedTypeArgs = GetTypeArgs(attribute.Signature!.NamedArguments.Select(a => a.Argument), resolver);
        }



        public void Fix()
        {
            _attribute.Constructor = (ICustomAttributeType)_resolved.GetResolved();
            ApplyArgs(_attribute.Signature!.FixedArguments, _fixedTypeArgs);
            ApplyArgs(_attribute.Signature!.NamedArguments.Select(a => a.Argument), _namedTypeArgs);
        }


        private IEnumerable<IEnumerable<IResolvedReference<TypeSignature>?>> GetTypeArgs(
            IEnumerable<CustomAttributeArgument> arguments, ReferenceResolver resolver)
        {
            return arguments
                .Select(a => a.Elements
                    .Select(el =>
                        (el is TypeSignature type) ? resolver.ResolveSig(type) : null
                    )
                    .ToList()
                )
                .ToList();
        }

        private void ApplyArgs(IEnumerable<CustomAttributeArgument> arguments,
            IEnumerable<IEnumerable<IResolvedReference<TypeSignature>?>> resolved)
        {
            foreach ((var original, var res) in arguments.Zip(resolved))
            {
                foreach ((int idx, var typeEl) in Enumerable.Range(0, original.Elements.Count).Zip(res))
                {
                    if (typeEl != null)
                        original.Elements[idx] = typeEl.GetResolved();
                }
            }
        }
    }
}
