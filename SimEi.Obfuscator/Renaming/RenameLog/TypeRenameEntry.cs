
using AsmResolver.DotNet;

namespace SimEi.Obfuscator.Renaming.RenameLog
{
    internal class TypeRenameEntry : RenameEntry
    {
        public TypeRenameEntry(TypeDefinition typeDef)
            : base(typeDef)
        {
            NestedTypes = new List<TypeRenameEntry>();
            Fields = new List<RenameEntry>();
            Properties = new List<RenameEntry>();
            Events = new List<RenameEntry>();
            Methods = new List<RenameEntry>();
        }


        public List<TypeRenameEntry> NestedTypes { get; }
        public List<RenameEntry> Fields { get; }
        public List<RenameEntry> Properties { get; }
        public List<RenameEntry> Events { get; }
        public List<RenameEntry> Methods { get; }


        public override void Serialize(TextWriter writer, int indentation)
        {
            base.WriteIndentation(writer, indentation);
            writer.Write("BeginType ");
            base.Serialize(writer, 0);

            WriteCollection(writer, "Fields", indentation + 1, Fields.Where(e => e.IsRenamed));
            WriteCollection(writer, "Props", indentation + 1, Properties.Where(e => e.IsRenamed));
            WriteCollection(writer, "Events", indentation + 1, Events.Where(e => e.IsRenamed));
            WriteCollection(writer, "Methods", indentation + 1, Methods.Where(e => e.IsRenamed));

            base.WriteLineWithIndentation(writer, indentation, "EndType");
        }

        private void WriteCollection(TextWriter writer, string name, int indentation, IEnumerable<RenameEntry> entries)
        {
            if (!entries.Any())
                return;

            base.WriteLineWithIndentation(writer, indentation, $"Begin{name}");
            foreach (var e in entries.OrderBy(e => e.Original))
                e.Serialize(writer, indentation + 1);
            base.WriteLineWithIndentation(writer, indentation, $"End{name}");
        }
    }
}
