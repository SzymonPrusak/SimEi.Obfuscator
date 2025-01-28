using AsmResolver.DotNet;
using AsmResolver.DotNet.Code.Cil;
using AsmResolver.PE.DotNet.Cil;
using SimEi.Obfuscator.Extension;
using SimEi.Obfuscator.Renaming.Reference.Resolving;
using SimEi.Obfuscator.Renaming.SigGraph;

namespace SimEi.Obfuscator.Renaming.Reference
{
    internal class ReferenceTracker : ModuleVisitorBase
    {
        private readonly IMetadataResolver _metadataResolver;
        private readonly ReferenceResolver _referenceResolver;

        private readonly MethodSigGraph _methodSigGraph;

        private readonly List<ITrackedReference> _definitionReferences;
        private readonly List<ITrackedReference> _referenceReferences;

        public ReferenceTracker(IMetadataResolver metadataResolver, MethodSigGraph methodSigGraph)
        {
            _metadataResolver = metadataResolver;
            _referenceResolver = new ReferenceResolver(metadataResolver);

            _methodSigGraph = methodSigGraph;

            _definitionReferences = new();
            _referenceReferences = new();
        }



        public int ReferenceCount => _definitionReferences.Count + _referenceReferences.Count;



        public void FixTrackedReferences()
        {
            foreach (var r in _definitionReferences)
                r.Fix();
            foreach (var r in _referenceReferences)
                r.Fix();
        }


        public override void VisitType(TypeDefinition type)
        {
            _definitionReferences.Add(new TypeSignatureReference(type, _referenceResolver));

            foreach (var gparam in type.GenericParameters)
                _definitionReferences.Add(new GenericParameterReference(gparam, _referenceResolver));

            VisitAttributes(type);

            // TODO: track default member (for indexer)

            for (int i = 0; i < type.MethodImplementations.Count; i++)
                _referenceReferences.Add(new MethodImplementationReference(type, i, _referenceResolver));

            if (!type.IsInterface)
            {
                // Tie methods that are possibly implementing implemented interface members in signature graph.

                var possiblyOverridingMethodsById = type.GetAllMethodDefinitions(_metadataResolver)
                    .Where(entry => !entry.Item2.IsStatic && entry.Item2.IsVirtual)
                    .GroupBy(entry => entry.Item1)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Item2)
                    );

                foreach ((var id, var toOverride) in type.GetInterfaceMethodsToImplement(_metadataResolver))
                {
                    if (possiblyOverridingMethodsById.TryGetValue(id, out var methods))
                    {
                        var toOverrideDef = _metadataResolver.ResolveMethod(toOverride)!;
                        foreach (var m in methods)
                            _methodSigGraph.Connect(m, toOverrideDef);
                    }
                }

                // Tie methods that are possibly overriding base classes methods in signature graph.

                var baseClassVirtualMethods = type.BaseType!.GetAllMethodDefinitions(_metadataResolver)
                    .Where(entry => !entry.Item2.IsStatic && entry.Item2.IsVirtual && !entry.Item2.IsFinal)
                    .GroupBy(entry => entry.Item1)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Item2)
                    );

                foreach (var overriding in type.Methods)
                {
                    if (overriding.IsStatic || overriding.IsNewSlot || !overriding.IsVirtual)
                        continue;

                    var id = new MethodIdentifier(overriding.Signature!, overriding.Name!);
                    if (baseClassVirtualMethods.TryGetValue(id, out var methods))
                    {
                        foreach (var m in methods)
                            _methodSigGraph.Connect(overriding, m);
                    }
                }
            }
        }

        public override void VisitField(FieldDefinition field)
        {
            _definitionReferences.Add(new FieldSignatureReference(field.Signature!, _referenceResolver));

            VisitAttributes(field);
        }

        public override void VisitProp(PropertyDefinition prop)
        {
            _definitionReferences.Add(new PropSignatureReference(prop.Signature!, _referenceResolver));

            VisitAttributes(prop);
        }

        public override void VisitEvent(EventDefinition evt)
        {
            _definitionReferences.Add(new EventDelegateReference(evt, _referenceResolver));

            VisitAttributes(evt);
        }

        public override void VisitMethod(MethodDefinition method)
        {
            _definitionReferences.Add(new MethodSignatureReference(method, _referenceResolver));

            foreach (var gparam in method.GenericParameters)
                _definitionReferences.Add(new GenericParameterReference(gparam, _referenceResolver));

            VisitAttributes(method);
        }

        public override void VisitLocal(CilLocalVariable local)
        {
            _definitionReferences.Add(new LocalSignatureReference(local, _referenceResolver));
        }

        public override void VisitInstruction(CilInstruction instruction)
        {
            if (instruction.Operand is MemberReference mr)
            {
                if (mr.IsMethod)
                    _referenceReferences.Add(new InstructionMethodReference(instruction, _referenceResolver));
                else
                    _referenceReferences.Add(new InstructionFieldReference(instruction, _referenceResolver));
            }
            else if (instruction.Operand is IMethodDescriptor)
            {
                _referenceReferences.Add(new InstructionMethodReference(instruction, _referenceResolver));
            }
            else if (instruction.Operand is ITypeDefOrRef type)
            {
                _referenceReferences.Add(new InstructionTypeReference(instruction, _referenceResolver));
            }
        }

        public override void VisitExceptionHandler(CilExceptionHandler excHandler)
        {
            _referenceReferences.Add(new ExceptionSignatureReference(excHandler, _referenceResolver));
        }


        private void VisitAttributes(IHasCustomAttribute member)
        {
            foreach (var attr in member.CustomAttributes)
                _referenceReferences.Add(new CustomAttributeReference(attr, _referenceResolver));
        }
    }
}
