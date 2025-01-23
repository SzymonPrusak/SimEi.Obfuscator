using AsmResolver.DotNet;
using AsmResolver.DotNet.Signatures;

namespace SimEi.Obfuscator.Renaming.Reference.Resolving
{
    internal class ResolvedMethodReference : IResolvedReference<IMethodDefOrRef>
    {
        private readonly IMethodDefOrRef _original;
        private readonly IMethodDefOrRef _resolved;

        public ResolvedMethodReference(IMethodDefOrRef original, IMethodDefOrRef resolved)
        {
            _original = original;
            _resolved = resolved;
        }


        public IMethodDefOrRef GetResolved()
        {
            var resolved = _resolved;
            if (_original.DeclaringType is TypeSpecification typeSpec)
            {
                var args = ((GenericInstanceTypeSignature)typeSpec.Signature!).TypeArguments;
                var genericInstance = _resolved.DeclaringType!.MakeGenericInstanceType([..args]);
                var resolvedTs = new TypeSpecification(genericInstance);
                resolved = new MemberReference(resolvedTs, _resolved.Name, _resolved.Signature);
            }
            return _original.Module!.DefaultImporter.ImportMethod(resolved);
        }
    }
}
