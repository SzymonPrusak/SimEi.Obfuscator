using AsmResolver.DotNet;

namespace SimEi.Obfuscator.Renaming.SigGraph
{
    internal class MethodNode
    {
        private MethodNode? _parent;
        private HashSet<MethodNode>? _children;

        public MethodNode(MethodDefinition method)
        {
            Method = method;
        }


        public MethodDefinition Method { get; }

        public bool IsMasterNode => _parent == null;

        public IEnumerable<MethodNode> ConnectedNodes
        {
            get => IsMasterNode
                ? (_children ?? Enumerable.Empty<MethodNode>()).Prepend(this)
                : _parent!.ConnectedNodes;
        }


        public static void Union(MethodNode n1, MethodNode n2)
        {
            var master = n1._parent ?? n1;
            var slaveMaster = n2._parent ?? n2;
            if (master == slaveMaster)
                return;

            slaveMaster._parent = master;
            master.AddChild(slaveMaster);

            if (slaveMaster._children != null)
            {
                foreach (var sc in slaveMaster._children)
                {
                    sc._parent = master;
                    master.AddChild(sc);
                }
            }

            slaveMaster._children = null;
        }


        private void AddChild(MethodNode n)
        {
            if (!IsMasterNode)
                throw new InvalidOperationException("Cannot have more than one level of depth.");
            _children ??= new HashSet<MethodNode>();
            _children.Add(n);
        }
    }
}
