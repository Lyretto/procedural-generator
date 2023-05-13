using System;
using UnityEngine;


namespace Lyred
{
    [Serializable]
    public class NodeSlot : IDisposable
    {
        [HideInInspector] [SerializeReference]
        public string guid;
        [HideInInspector] [SerializeReference]
        public string name;
        [HideInInspector] [SerializeReference]
        public string slotTypeName;
        [HideInInspector] [SerializeReference]
        public Node node;
        [HideInInspector] [SerializeReference]
        public NodeSlot parentNodeSlot;
        public object defaultValue;

        public object InvokeNode()
        {
            return parentNodeSlot?.node.Result();
        }
        
        public NodeSlot(Node node, string displayName, string type)
        {
            this.node = node;
            name = displayName;
            slotTypeName = type;
        }

        public bool AddParent(NodeSlot parent)
        {
            Debug.Log("Types:" + parent.slotTypeName + "  " + slotTypeName);
            if (parent.slotTypeName != slotTypeName) return false;
            parentNodeSlot = parent;
            return true;
        }
        
        public void Dispose()
        {
            Debug.Log("nodeSlot disposed.");
        }
    }
}