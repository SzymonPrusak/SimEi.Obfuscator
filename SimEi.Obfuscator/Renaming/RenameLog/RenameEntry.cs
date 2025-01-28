using AsmResolver.DotNet;

namespace SimEi.Obfuscator.Renaming.RenameLog
{
    internal class RenameEntry
    {
        private readonly IMemberDefinition _member;

        public RenameEntry(IMemberDefinition member)
        {
            _member = member;

            Original = Current;
        }


        public string Original { get; }
        public string Current => _member is TypeDefinition typeDef ? typeDef.FullName! : _member.Name!;

        public bool IsRenamed => Original != Current;


        public virtual void Serialize(TextWriter writer, int indentation)
        {
            WriteIndentation(writer, indentation);
            if (!IsRenamed)
                writer.WriteLine(Original);
            else
            {
                writer.Write(Original);
                writer.Write(" => ");
                writer.WriteLine(Current);
            }
        }

        protected void WriteLineWithIndentation(TextWriter writer, int indentation, string text)
        {
            WriteIndentation(writer, indentation);
            writer.WriteLine(text);
        }

        protected void WriteIndentation(TextWriter writer, int indentation)
        {
            for (int i = 0; i < indentation; i++)
                writer.Write('\t');
        }
    }
}
