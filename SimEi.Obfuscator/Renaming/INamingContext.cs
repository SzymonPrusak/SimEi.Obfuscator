using AsmResolver.DotNet;

namespace SimEi.Obfuscator.Renaming
{
    internal interface INamingContext
    {
        string GetNextName(TypeDefinition? type, RenamedElementType elType);
    }
}
