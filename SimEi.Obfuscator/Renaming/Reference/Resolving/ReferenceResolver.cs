using AsmResolver.DotNet;
using AsmResolver.DotNet.Signatures;

namespace SimEi.Obfuscator.Renaming.Reference.Resolving
{
    internal class ReferenceResolver
    {
        private readonly IMetadataResolver _metadataResolver;

        private readonly Dictionary<ITypeDefOrRef, IResolvedReference<ITypeDefOrRef>> _typeCache;
        private readonly Dictionary<IMethodDescriptor, IResolvedReference<IMethodDescriptor>> _methodCache;
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


        public IResolvedReference<IMethodDescriptor> Resolve(IMethodDescriptor method)
        {
            if (_methodCache.TryGetValue(method, out var r))
                return r;
            return _methodCache[method] = ResolveCore(method);
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
            GenericInstanceTypeSignature? genSig = null;
            if (type is TypeSpecification spec)
            {
                if (spec.Signature is GenericParameterSignature)
                    return new NoResolveReference<ITypeDefOrRef>(type);

                genSig = spec.Signature as GenericInstanceTypeSignature;
            }

            var resolved = _metadataResolver.ResolveType(type);
            if (resolved == null)
                throw new ArgumentException();

            if (genSig != null)
            {
                var gargs = genSig.TypeArguments
                    .Select(ResolveSigCore)
                    .ToList();
                return new ResolvedTypeReference(type, resolved, gargs);
            }
            return new ResolvedTypeReference(type, resolved);
        }


        private IResolvedReference<IMethodDescriptor> ResolveCore(IMethodDescriptor method)
        {
            var resolved = _metadataResolver.ResolveMethod(method);
            if (resolved == null)
                throw new ArgumentException();

            // TODO: method signatures need to be resolved as well - they can have types from another assemblies
            //  that won't be updated automatically.
            IEnumerable<IResolvedReference<TypeSignature>>? gtArgs = null;
            if (method.DeclaringType is TypeSpecification tSpec && tSpec.Signature is GenericInstanceTypeSignature gtSig)
            {
                gtArgs = gtSig.TypeArguments
                    .Select(ResolveSigCore)
                    .ToList();
            }
            IEnumerable<IResolvedReference<TypeSignature>>? gmArgs = null;
            if (method is MethodSpecification mSpec)
            {
                gmArgs = mSpec.Signature!.TypeArguments
                    .Select(ResolveSigCore)
                    .ToList();
            }
            return new ResolvedMethodReference(method, resolved, gtArgs, gmArgs);
        }


        private IResolvedReference<IFieldDescriptor> ResolveCore(IFieldDescriptor field)
        {
            var resolved = _metadataResolver.ResolveField(field);
            if (resolved == null)
                throw new ArgumentException();

            // TODO: field signatures need to be resolved as well.
            return new ResolvedFieldReference(field, resolved);
        }


        private IResolvedReference<TypeSignature> ResolveSigCore(TypeSignature sig)
        {
            var rootSig = GetRootSignature(sig);
            if (rootSig is GenericParameterSignature)
                return new ResolvedSignatureReference(sig, null, null);

            var resolved = _metadataResolver.ResolveType(sig);

            if (resolved == null)
                throw new ArgumentException();

            var gargs = (rootSig as GenericInstanceTypeSignature)?.TypeArguments
                .Select(ResolveSigCore)
                .ToList();
            return new ResolvedSignatureReference(sig, resolved, gargs);
        }

        private TypeSignature? GetRootSignature(TypeSignature sig)
        {
            switch (sig)
            {
                case CorLibTypeSignature:
                case TypeDefOrRefSignature:
                case GenericParameterSignature:
                case GenericInstanceTypeSignature:
                case FunctionPointerTypeSignature:
                case SentinelTypeSignature:
                    return sig;

                case TypeSpecificationSignature arr:
                    return GetRootSignature(arr.BaseType);
            }
            throw new ArgumentException();
        }
    }
}
