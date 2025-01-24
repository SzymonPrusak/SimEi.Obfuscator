using AsmResolver.DotNet;
using AsmResolver.DotNet.Signatures;

namespace SimEi.Obfuscator.Renaming.Reference.Resolving
{
    internal class ReferenceResolver
    {
        private readonly IMetadataResolver _metadataResolver;

        private readonly Dictionary<ITypeDefOrRef, IResolvedReference<ITypeDefOrRef>> _typeCache;
        private readonly Dictionary<IMethodDefOrRef, IResolvedReference<IMethodDefOrRef>?> _methodCache;
        private readonly Dictionary<IFieldDescriptor, IResolvedReference<IFieldDescriptor>> _fieldCache;
        private readonly Dictionary<TypeSignature, IResolvedReference<TypeSignature>> _sigCache;

        public ReferenceResolver(IMetadataResolver metadataResolver)
        {
            _metadataResolver = metadataResolver;

            _typeCache = new();
            _methodCache = new();
            _fieldCache = new();
            _sigCache = new();
        }



        public IResolvedReference<ITypeDefOrRef> Resolve(ITypeDefOrRef type)
        {
            if (_typeCache.TryGetValue(type, out var r))
                return r;
            return _typeCache[type] = ResolveCore(type);
        }


        public IResolvedReference<IMethodDefOrRef> Resolve(IMethodDefOrRef method)
        {
            return TryResolveCore(method) ?? throw new ArgumentException();
        }

        public IResolvedReference<IMethodDefOrRef>? TryResolve(IMethodDefOrRef method)
        {
            if (_methodCache.TryGetValue(method, out var r))
                return r;
            return _methodCache[method] = TryResolveCore(method);
        }


        public IResolvedReference<IFieldDescriptor> Resolve(IFieldDescriptor field)
        {
            if (_fieldCache.TryGetValue(field, out var r))
                return r;
            return _fieldCache[field] = ResolveCore(field);
        }


        public IResolvedReference<TypeSignature> ResolveSig(TypeSignature sig)
        {
            if (_sigCache.TryGetValue(sig, out var r))
                return r;
            return _sigCache[sig] = ResolveSigCore(sig);
        }


        private IResolvedReference<ITypeDefOrRef> ResolveCore(ITypeDefOrRef type)
        {
            var resolved = _metadataResolver.ResolveType(type);
            if (resolved == null)
                throw new ArgumentException();

            if (type is TypeSpecification spec && spec.Signature is GenericInstanceTypeSignature sig)
            {
                var gargs = sig.TypeArguments
                    .Select(ResolveSigCore)
                    .ToList();
                return new ResolvedTypeReference(type, resolved, gargs);
            }
            return new ResolvedTypeReference(type, resolved);
        }


        private IResolvedReference<IMethodDefOrRef>? TryResolveCore(IMethodDefOrRef method)
        {
            var resolved = _metadataResolver.ResolveMethod(method);
            if (resolved == null)
                return null;

            return new ResolvedMethodReference(method, resolved);
        }


        private IResolvedReference<IFieldDescriptor> ResolveCore(IFieldDescriptor field)
        {
            var resolved = _metadataResolver.ResolveField(field);
            if (resolved == null)
                throw new ArgumentException();

            return new ResolvedFieldReference(field, resolved);
        }


        private IResolvedReference<TypeSignature> ResolveSigCore(TypeSignature sig)
        {
            var resolved = _metadataResolver.ResolveType(sig);
            if (resolved == null)
                throw new ArgumentException();

            var gitSig = TraverseToGeneric(sig);
            var gargs = gitSig?.TypeArguments
                .Select(ResolveSigCore)
                .ToList();
            return new ResolvedSignatureReference(sig, resolved, gargs);

            //return new NoResolveReference<TypeSignature>(sig);
        }

        private GenericInstanceTypeSignature? TraverseToGeneric(TypeSignature sig)
        {
            switch (sig)
            {
                case CorLibTypeSignature:
                case TypeDefOrRefSignature:
                case GenericParameterSignature:
                    return null;
                // TODO 
                //case FunctionPointerTypeSignature fpSig:
                //case SentinelParameterTypeSignature:
                case GenericInstanceTypeSignature generic:
                    return generic;
                case TypeSpecificationSignature arr:
                    return TraverseToGeneric(arr.BaseType);
            }
            throw new ArgumentException();
        }
    }
}
