using System.Collections.Generic;
using UnityEngine;

namespace Lyred
{
    [CreateAssetMenu()]
    public class NodeGraph : ScriptableObject {

        [SerializeReference] public Node rootNode;
        [HideInInspector] public Node currentNode;
        [SerializeReference] public List<Node> nodes = new ();
        public Blackboard blackboard = new ();
        
        [HideInInspector] public Vector3 viewPosition = new (600, 300);
        [HideInInspector]  public Vector3 viewScale = Vector3.one;

        public void Generate()
        {
            var currentNode = this.currentNode ?? rootNode;
            currentNode?.Result();
        }

        public static void Traverse(Node node, System.Action<Node> visiter)
        {
            if (node == null) return;
            
            visiter.Invoke(node);
            var children = node.inputPorts;
            children.ForEach(n=> Traverse(n.parentNodeSlot?.node, visiter));
        }

        public NodeGraph Clone() {
            return Instantiate(this);
        }
    }
}
