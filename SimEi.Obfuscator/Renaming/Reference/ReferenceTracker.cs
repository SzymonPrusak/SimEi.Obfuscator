using AsmResolver.DotNet;
using AsmResolver.DotNet.Code.Cil;
using AsmResolver.PE.DotNet.Cil;
using SimEi.Obfuscator.Extension;
using SimEi.Obfuscator.Renaming.Reference.Resolving;
using SimEi.Obfuscator.Renaming.SigGraph;

namespace SimEi.Obfuscator.Renaming.Reference
{
    // TODO: track member attributes
    internal class ReferenceTracker : ModuleVisitorBase
    {
        private readonly IMetadataResolver _metadataResolver;

        private readonly MethodSigGraph _methodSigGraph;

        private readonly List<ITrackedReference> _trackedReferences;

        public ReferenceTracker(IMetadataResolver metadataResolver, MethodSigGraph methodSigGraph)
        {
            _metadataResolver = metadataResolver;

            _methodSigGraph = methodSigGraph;

            _trackedReferences = [];
        }



        public void FixTrackedReferences()
        {
            foreach (var r in _trackedReferences)
                r.Fix();
        }


        public override void VisitType(TypeDefinition type, IReadOnlyList<TypeDefinition> declaringTypes)
        {
            _trackedReferences.Add(new TypeSignatureReference(type));

            foreach (var gparam in type.GenericParameters)
                _trackedReferences.Add(new GenericParameterReference(gparam));

            VisitAttributes(type);

            // TODO: track default member (for indexer)

            for (int i = 0; i < type.MethodImplementations.Count; i++)
                _trackedReferences.Add(new MethodImplementationReference(type, i));

            if (!type.IsInterface)
            {
                var possiblyOverridingMethodsById = type.GetAllMethodDefinitions(_metadataResolver)
                    .Where(entry => !entry.Item2.IsStatic && entry.Item2.IsVirtual)
                    .GroupBy(entry => entry.Item1)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Item2)
                    );

                // Tie methods that are possibly implementing implemented interface members in signature graph.
                foreach ((var id, var toOverride) in type.GetInterfaceMethodsToImplement(_metadataResolver))
                {
                    if (possiblyOverridingMethodsById.TryGetValue(id, out var methods))
                    {
                        var toOverrideDef = (MethodDefinition)toOverride.Resolve()!;
                        foreach (var m in methods)
                            _methodSigGraph.Connect(m, toOverrideDef);
                    }
                }

                // Tie methods that are possibly overriding base classes methods in signature graph.
                foreach (var method in type.Methods)
                {
                    if (method.IsStatic || method.IsNewSlot || !method.IsVirtual)
                        continue;

                    var bc = ReferenceResolver.Resolve(type.BaseType!).GetResolved();
                    while (bc != null)
                    {
                        var methodRef = new MemberReference(bc, method.Name, method.Signature);
                        var bcMethod = (MethodDefinition?)methodRef.Resolve();

                        if (bcMethod != null && !bcMethod.IsStatic && bcMethod.IsVirtual && !bcMethod.IsFinal)
                        {
                            _methodSigGraph.Connect(method, bcMethod);
                            break;
                        }
                    }
                }
            }
        }

        public override void VisitField(FieldDefinition field, IReadOnlyList<TypeDefinition> declaringTypes)
        {
            _trackedReferences.Add(new FieldSignatureReference(field.Signature!));

            VisitAttributes(field);
        }

        public override void VisitProp(PropertyDefinition prop, IReadOnlyList<TypeDefinition> declaringTypes)
        {
            _trackedReferences.Add(new PropSignatureReference(prop.Signature!));

            VisitAttributes(prop);
        }

        public override void VisitEvent(EventDefinition evt, IReadOnlyList<TypeDefinition> declaringTypes)
        {
            _trackedReferences.Add(new EventDelegateReference(evt));

            VisitAttributes(evt);
        }

        public override void VisitMethod(MethodDefinition method, IReadOnlyList<TypeDefinition> declaringTypes)
        {
            _trackedReferences.Add(new MethodSignatureReference(method));

            foreach (var gparam in method.GenericParameters)
                _trackedReferences.Add(new GenericParameterReference(gparam));

            VisitAttributes(method);
        }

        public override void VisitLocal(CilLocalVariable local)
        {
            _trackedReferences.Add(new LocalSignatureReference(local));
        }

        public override void VisitInstruction(CilInstruction instruction)
        {
            if (instruction.Operand is MemberReference mr)
            {
                if (mr.IsMethod)
                    _trackedReferences.Add(new InstructionMethodReference(instruction));
                else
                    _trackedReferences.Add(new InstructionFieldReference(instruction));
            }
            else if (instruction.Operand is MethodSpecification)
            {
                _trackedReferences.Add(new InstructionMethodSpecReference(instruction));
            }
        }

        public override void VisitExceptionHandler(CilExceptionHandler excHandler)
        {
            _trackedReferences.Add(new ExceptionSignatureReference(excHandler));
        }


        private void VisitAttributes(IHasCustomAttribute member)
        {
            foreach (var attr in member.CustomAttributes)
                _trackedReferences.Add(new CustomAttributeReference(attr));
        }
    }
}
