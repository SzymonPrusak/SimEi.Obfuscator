using AsmResolver.DotNet;
using SimEi.Obfuscator.Config;

namespace SimEi.Obfuscator.Renaming.Permission.Config
{
    internal class ConfigPermissions : IRenamingPermissions
    {
        private readonly IEnumerable<RuleNode> _globalRules;
        private readonly Dictionary<string, IEnumerable<RuleNode>> _moduleRules;

        private readonly IMetadataResolver _metadataResolver;

        public ConfigPermissions(ConfigDocument config, IMetadataResolver metadataResolver)
        {
            _globalRules = CreateRuleNodes(config.GlobalRules);
            _moduleRules = config.Modules
                .ToDictionary(
                    a => a.Name,
                    a => CreateRuleNodes(a.Rules)
                );

            _metadataResolver = metadataResolver;
        }



        public bool CanRename(IMetadataMember member)
        {
            var module = member switch
            {
                IMemberDefinition def => def.Module!,
                ParameterDefinition pd => pd.Method!.Module!,
                _ => throw new ArgumentException()
            };

            var rules = _globalRules;
            string moduleName = module.Name!.ToString().Trim('"');
            if (_moduleRules.TryGetValue(moduleName, out var moduleRules))
                rules = rules.Concat(moduleRules);

            var res = rules.Aggregate((ActionType?)null, (at, r) =>
            {
                if (at.HasValue && at.Value == ActionType.Include)
                    return ActionType.Include;
                return r.ResolveAction(member, _metadataResolver) ?? at;
            });
            return (res ?? ActionType.Include) == ActionType.Include;
        }


        private IEnumerable<RuleNode> CreateRuleNodes(IEnumerable<Rule> rules)
        {
            var rootRules = rules
                .Select(r => new RuleNode(r, null))
                .ToList();
            return rootRules
                .SelectMany(r => r.GetAllSubRules())
                .ToList();
        }
    }
}
