using System.Xml.Serialization;
using AsmResolver.DotNet;
using AsmResolver.DotNet.Serialized;
using SimEi.Obfuscator.Config;
using SimEi.Obfuscator.Renaming;
using SimEi.Obfuscator.Renaming.Permission.Config;
using SimEi.Obfuscator.Renaming.RenameLog;

namespace SimEi.Obfuscator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Error: no config specified.");
                Environment.Exit(1);
            }
            var config = ReadConfig(args[0]);
            if (config == null)
            {
                Console.WriteLine("Error: invalid config file.");
                Environment.Exit(1);
            }

            var rtInfo = new DotNetRuntimeInfo(DotNetRuntimeInfo.NetFramework, new Version("4.8.1"));
            var rtCtx = new RuntimeContext(rtInfo);
            var mrp = new ModuleReaderParameters(rtCtx);

            string basePath = config.BaseDir!.Value;
            var asms = config.Modules.Select(a => a.Name)
                .Select(p => AssemblyDefinition.FromFile(Path.Combine(basePath, p), mrp))
                .ToList();

            var asmResolver = new ObfuscatedAssemblyResolver(asms);
            asmResolver.SearchDirectories.Add(basePath);
            foreach (var pp in config.ProbePaths)
                asmResolver.SearchDirectories.Add(pp.Value);
            var metadataResolver = new DefaultMetadataResolver(asmResolver);

            var modules = asms.SelectMany(a => a.Modules);
            var configPerm = new ConfigPermissions(config, metadataResolver);
            var renaming = new RenamingPipeline(metadataResolver, configPerm);

            var logger = new RenamingLogger();
            renaming.Rename(modules, logger);

            Logger.Log($"Saving output files to {Environment.CurrentDirectory}");
            foreach (var lib in asms)
                lib.Write(lib.Name + ".dll");

            string mappingFilePath = Path.Combine(Environment.CurrentDirectory, "mapping.obf");
            Logger.Log($"Saving mapping file to {mappingFilePath}");
            using (var fstream = File.Create(mappingFilePath))
            using (var writer = new StreamWriter(fstream))
            {
                foreach (var type in logger.TrackedGlobalTypes.OrderBy(e => e.Original))
                    type.Serialize(writer, 0);
            }
        }


        private static ConfigDocument? ReadConfig(string configPath)
        {
            ConfigDocument? config;
            var serialier = new XmlSerializer(typeof(ConfigDocument));
            using (var fstream = File.OpenRead(configPath))
            {
                config = (ConfigDocument?)serialier.Deserialize(fstream);
            }
            return config;
        }
    }
}
