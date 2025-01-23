using AsmResolver.DotNet;
using SimEi.Obfuscator.Renaming.Reference.Resolving;

namespace SimEi.Obfuscator.Renaming.Reference
{
    internal class GenericParameterReference : ITrackedReference
    {
        private readonly GenericParameter _parameter;

        private readonly IEnumerable<IResolvedReference<ITypeDefOrRef>> _constraintRefs;

        public GenericParameterReference(GenericParameter parameter)
        {
            _parameter = parameter;

            _constraintRefs = parameter.Constraints
                .Select(c => ReferenceResolver.Resolve(c.Constraint!))
                .ToList();
        }


        public void Fix()
        {
            foreach ((var constraint, var r) in _parameter.Constraints.Zip(_constraintRefs))
                constraint.Constraint = r.GetResolved();
        }
    }
}
