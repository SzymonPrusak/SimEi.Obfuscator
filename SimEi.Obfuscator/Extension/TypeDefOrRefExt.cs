using AsmResolver.DotNet;
using AsmResolver.DotNet.Signatures;

namespace SimEi.Obfuscator.Extension
{
    internal static class TypeDefOrRefExt
    {
        public static List<(MethodIdentifier, MethodDefinition)> GetAllMethodDefinitions(this ITypeDefOrRef type,
            IMetadataResolver resolver)
        {
            var methods = new List<(MethodIdentifier, MethodDefinition)>();

            ITypeDefOrRef? curDefOrSpec = type;
            while (curDefOrSpec != null)
            {
                GenericContext? genCtx = null;
                if (curDefOrSpec is TypeSpecification spec)
                {
                    var specSig = (GenericInstanceTypeSignature)spec.Signature!;
                    genCtx = new GenericContext(specSig, null);
                }

                var typeDef = resolver.ResolveType(curDefOrSpec)!;
                foreach (var method in typeDef.Methods)
                {
                    if (genCtx.HasValue)
                    {
                        var genSig = method.Signature!.InstantiateGenericTypes(genCtx.Value);
                        methods.Add((new MethodIdentifier(genSig, method.Name!), method));
                    }
                    else
                    {
                        methods.Add((new MethodIdentifier(method.Signature!, method.Name!), method));
                    }
                }

                if (genCtx.HasValue && typeDef.BaseType is TypeSpecification baseSpec)
                {
                    var baseSpecSig = (GenericInstanceTypeSignature)baseSpec.Signature!;
                    var baseSig = baseSpecSig.InstantiateGenericTypes(genCtx.Value);
                    curDefOrSpec = new TypeSpecification(baseSig);
                }
                else
                {
                    curDefOrSpec = typeDef.BaseType!;
                }
            }

            return methods;
        }


        public static List<(MethodIdentifier, MemberReference)> GetInterfaceMethodsToImplement(this TypeDefinition type,
            IMetadataResolver resolver)
        {
            var methods = new List<(MethodIdentifier, MemberReference)>();
            foreach (var iface in type.Interfaces)
                AddMethods(iface.Interface!, resolver, methods);
            return methods;
        }

        private static void AddMethods(ITypeDefOrRef iface, IMetadataResolver resolver,
            List<(MethodIdentifier, MemberReference)> res)
        {
            var ifaceDef = resolver.ResolveType(iface)!;
            foreach (var method in ifaceDef.Methods)
            {
                var r = new MemberReference(iface, method.Name, method.Signature);

                if (iface is TypeSpecification ifaceSpec)
                {
                    var genIfaceSig = (GenericInstanceTypeSignature)ifaceSpec.Signature!;
                    var genericCtx = new GenericContext(genIfaceSig, null);
                    var genSig = method.Signature!.InstantiateGenericTypes(genericCtx);
                    res.Add((new MethodIdentifier(genSig, method.Name!), r));
                }
                else
                {
                    res.Add((new MethodIdentifier(method.Signature!, method.Name!), r));
                }
            }
        }
    }
}
