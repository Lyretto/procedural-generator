using System.Collections.Generic;

namespace Lyred
{
    public abstract class CompositeNode : Node
    {
        public List<Node> children = new();
    }

}