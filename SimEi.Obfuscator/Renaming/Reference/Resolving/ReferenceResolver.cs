using AsmResolver.DotNet;
using AsmResolver.DotNet.Signatures;

namespace SimEi.Obfuscator.Renaming.Reference.Resolving
{
    internal static class ReferenceResolver
    {
        public static IResolvedReference<ITypeDefOrRef> Resolve(ITypeDefOrRef type)
        {
            var resolved = type.Resolve();
            if (resolved == null)
                throw new ArgumentException();

            if (type is TypeSpecification spec)
            {
                var gargs = ((GenericInstanceTypeSignature)spec.Signature!).TypeArguments
                    .Select(ResolveSig)
                    .ToList();
                return new ResolvedTypeReference(type, resolved, gargs);
            }
            return new ResolvedTypeReference(type, resolved);
        }


        public static IResolvedReference<IMethodDefOrRef>? TryResolve(IMethodDefOrRef method)
        {
            var resolved = method.Resolve();
            if (resolved == null)
                return null;

            return new ResolvedMethodReference(method, resolved);
        }

        public static IResolvedReference<IMethodDefOrRef> Resolve(IMethodDefOrRef method)
        {
            return TryResolve(method) ?? throw new ArgumentException();
        }


        public static IResolvedReference<IFieldDescriptor> Resolve(IFieldDescriptor field)
        {
            var resolved = field.Resolve();
            if (resolved == null)
                throw new ArgumentException();

            return new ResolvedFieldReference(field, resolved);
        }


        public static IResolvedReference<TypeSignature> ResolveSig(TypeSignature sig)
        {
            var resolved = sig.Resolve();
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
