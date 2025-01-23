using AsmResolver.DotNet;

namespace SimEi.Obfuscator.Renaming.SigGraph
{
    internal class MethodSigGraph
    {
        private Dictionary<MethodDefinition, MethodNode> _nodes;

        public MethodSigGraph()
        {
            _nodes = new Dictionary<MethodDefinition, MethodNode>();
        }



        public IEnumerable<MethodDefinition> Nodes => _nodes.Keys;



        public void Connect(MethodDefinition m1, MethodDefinition m2)
        {
            MethodNode.Union(GetNode(m1), GetNode(m2));
        }

        public IEnumerable<SigComponent> GetComponents()
        {
            return _nodes.Values
                .Where(n => n.IsMasterNode)
                .Select(n => new SigComponent(n));
        }


        private MethodNode GetNode(MethodDefinition m)
        {
            if (!_nodes.TryGetValue(m, out var node))
            {
                node = new MethodNode(m);
                _nodes.Add(m, node);
            }
            return node;
        }
    }
}
