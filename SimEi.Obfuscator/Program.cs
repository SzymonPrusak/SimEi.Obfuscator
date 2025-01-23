using AsmResolver.DotNet;
using SimEi.Obfuscator.Renaming;

namespace SimEi.Obfuscator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var asm = AssemblyDefinition.FromFile(@"D:\Opensource\ConfuserExTest\bin\Debug\net481\ConfuserExTest.exe");
            var lib = AssemblyDefinition.FromFile(@"D:\Opensource\ConfuserExTest\bin\Debug\net481\Otherlib.dll");

            var asmResolver = new DotNetFrameworkAssemblyResolver();
            var metadataResolver = new DefaultMetadataResolver(asmResolver);

            var modules = new [] { asm, lib }.SelectMany(a => a.Modules);
            var renaming = new RenamingPipeline(metadataResolver);
            renaming.Rename(modules);

            asm.Write("ConfuserExTest-obf.exe");
            lib.Write("Otherlib.dll");
        }
    }
}
