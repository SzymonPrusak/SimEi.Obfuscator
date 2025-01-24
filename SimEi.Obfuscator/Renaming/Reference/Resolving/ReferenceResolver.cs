using AsmResolver.DotNet;
using AsmResolver.DotNet.Signatures;

namespace SimEi.Obfuscator.Renaming.Reference.Resolving
{
    internal class ReferenceResolver
    {
        private readonly IMetadataResolver _metadataResolver;

        public ReferenceResolver(IMetadataResolver metadataResolver)
        {
            _metadataResolver = metadataResolver;
        }


        public IResolvedReference<ITypeDefOrRef> Resolve(ITypeDefOrRef type)
        {
            var resolved = _metadataResolver.ResolveType(type);
            if (resolved == null)
                throw new ArgumentException();

            if (type is TypeSpecification spec && spec.Signature is GenericInstanceTypeSignature sig)
            {
                var gargs = sig.TypeArguments
                    .Select(ResolveSig)
                    .ToList();
                return new ResolvedTypeReference(type, resolved, gargs);
            }
            return new ResolvedTypeReference(type, resolved);
        }


        public IResolvedReference<IMethodDefOrRef>? TryResolve(IMethodDefOrRef method)
        {
            var resolved = _metadataResolver.ResolveMethod(method);
            if (resolved == null)
                return null;

            return new ResolvedMethodReference(method, resolved);
        }

        public IResolvedReference<IMethodDefOrRef> Resolve(IMethodDefOrRef method)
        {
            return TryResolve(method) ?? throw new ArgumentException();
        }


        public IResolvedReference<IFieldDescriptor> Resolve(IFieldDescriptor field)
        {
            var resolved = _metadataResolver.ResolveField(field);
            if (resolved == null)
                throw new ArgumentException();

            return new ResolvedFieldReference(field, resolved);
        }


        public IResolvedReference<TypeSignature> ResolveSig(TypeSignature sig)
        {
            var resolved = _metadataResolver.ResolveType(sig);
            if (resolved != null)
            {
                if (sig is GenericInstanceTypeSignature git)
                {
                    var gargs = git.TypeArguments
                        .Select(ResolveSig)
                        .ToList();
                    return new ResolvedSignatureReference(sig, resolved, gargs);
                }
                return new ResolvedSignatureReference(sig, resolved);
            }

            return new NoResolveReference<TypeSignature>(sig);
        }
    }
}
