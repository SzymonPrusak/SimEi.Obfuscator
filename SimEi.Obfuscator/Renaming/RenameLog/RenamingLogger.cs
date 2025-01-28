using AsmResolver.DotNet;

namespace SimEi.Obfuscator.Renaming.RenameLog
{
    internal class RenamingLogger : IRenamingLogger
    {
        private readonly List<TypeRenameEntry> _globalTypes;
        private readonly Dictionary<TypeDefinition, TypeRenameEntry> _typeEntries;

        public RenamingLogger()
        {
            _globalTypes = new List<TypeRenameEntry>();
            _typeEntries = new Dictionary<TypeDefinition, TypeRenameEntry>();
        }


        public IEnumerable<TypeRenameEntry> TrackedGlobalTypes => _globalTypes;


        public void Track(IMemberDefinition definition)
        {
            if (definition is TypeDefinition typeDef)
                GetOrCreateTypeEntry(typeDef);
            else
            {
                var typeEntry = GetOrCreateTypeEntry(definition.DeclaringType!);

                var entry = new RenameEntry(definition);
                var targetCol = definition switch
                {
                    FieldDefinition => typeEntry.Fields,
                    PropertyDefinition => typeEntry.Properties,
                    EventDefinition => typeEntry.Events,
                    MethodDefinition => typeEntry.Methods,
                    _ => throw new ArgumentException()
                };
                targetCol.Add(entry);
            }
        }

        private TypeRenameEntry GetOrCreateTypeEntry(TypeDefinition typeDef)
        {
            if (!_typeEntries.TryGetValue(typeDef, out var typeEntry))
            {
                typeEntry = new TypeRenameEntry(typeDef);
                if (typeDef.DeclaringType != null)
                    GetOrCreateTypeEntry(typeDef.DeclaringType).NestedTypes.Add(typeEntry);
                else
                    _globalTypes.Add(typeEntry);
                _typeEntries.Add(typeDef, typeEntry);
            }
            return typeEntry;
        }
    }
}
