using AsmResolver.DotNet;

namespace SimEi.Obfuscator.Renaming.RenameLog
{
    internal interface IRenamingLogger
    {
        void Track(IMemberDefinition definition);
    }
}
