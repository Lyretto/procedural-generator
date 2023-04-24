using System.Collections.Generic;
using UnityEngine;

namespace Lyred
{
    [CreateAssetMenu()]
    public class NodeGraph : ScriptableObject {

        [SerializeReference]
        public RootNode rootNode;

        [SerializeReference]
        public List<Node> nodes = new ();

        public Node.State treeState = Node.State.Running;
        public Blackboard blackboard = new ();

        #region  EditorProperties 
        public Vector3 viewPosition = new (600, 300);
        public Vector3 viewScale = Vector3.one;
        #endregion

        public NodeGraph()
        {
            rootNode = new RootNode();
            nodes.Add(rootNode);
        }

        public void Generate()
        {
            rootNode.Result();
        }

        public static void Traverse(Node node, System.Action<Node> visiter)
        {
            if (node == null) return;
            
            visiter.Invoke(node);
            var children = node.inputPorts;
            children.ForEach(n=> Traverse(n.parentNodeSlot?.node, visiter));
        }
        
        public static void TraverseChildren(Node node, System.Action<Node> visiter)
        {
            if (node == null) return;
            
            visiter.Invoke(node);
            var children = node.outputPorts;
            children.ForEach(n=> Traverse(n.parentNodeSlot?.node, visiter));
        }

        public NodeGraph Clone() {
            return Instantiate(this);
        }

        public void Bind(Context context) {
            Traverse(rootNode, node => {
                node.context = context;
                node.blackboard = blackboard;
            });
        }
    }

}
