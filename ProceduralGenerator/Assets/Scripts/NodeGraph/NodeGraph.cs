using System.Collections.Generic;
using UnityEngine;

namespace Lyred
{
    [CreateAssetMenu()]
    public class NodeGraph : ScriptableObject {

        [SerializeReference] public Node rootNode;
        [HideInInspector] public Node currentNode;
        [SerializeReference] public List<Node> nodes = new ();
        [SerializeReference] public Blackboard blackboard = new ();
        [HideInInspector] public GameObject parentObject;
        [HideInInspector] public Vector3 viewPosition = new (600, 300);
        [HideInInspector]  public Vector3 viewScale = Vector3.one;

        public void Generate(GameObject parentGameObject)
        {
            parentObject = parentGameObject;
            
            if(!parentGameObject) return;

            foreach (Transform child in parentObject.transform)
            {
                DestroyImmediate(child.gameObject);
            }
            
            if (!parentObject)
            {
                Debug.LogWarning("no Runner attached, generate new temporary parent object.");
                parentObject = new GameObject();
            }
            var node = currentNode ?? rootNode;
            
            Traverse(node, (n) => { n.parentObject = parentObject;});
            
            node?.Result();
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
