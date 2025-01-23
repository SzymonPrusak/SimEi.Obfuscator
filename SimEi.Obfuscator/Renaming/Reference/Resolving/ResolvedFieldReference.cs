using AsmResolver.DotNet;
using AsmResolver.DotNet.Signatures;

namespace SimEi.Obfuscator.Renaming.Reference.Resolving
{
    internal class ResolvedFieldReference : IResolvedReference<IFieldDescriptor>
    {
        private readonly IFieldDescriptor _original;
        private readonly IFieldDescriptor _resolved;

        public ResolvedFieldReference(IFieldDescriptor original, IFieldDescriptor resolved)
        {
            _original = original;
            _resolved = resolved;
        }


        public IFieldDescriptor GetResolved()
        {
            var resolved = _resolved;
            if (_original.DeclaringType is TypeSpecification typeSpec)
            {
                var args = ((GenericInstanceTypeSignature)typeSpec.Signature!).TypeArguments;
                var genericInstance = _resolved.DeclaringType!.MakeGenericInstanceType(args.ToArray());
                var resolvedTs = new TypeSpecification(genericInstance);
                resolved = new MemberReference(resolvedTs, _resolved.Name, _resolved.Signature);
            }
            return _original.Module!.DefaultImporter.ImportField(resolved);
        }
    }
}
