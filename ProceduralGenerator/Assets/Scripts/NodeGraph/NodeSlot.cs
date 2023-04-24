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
        public Type SlotType;
        [HideInInspector] [SerializeReference]
        public Node node;
        [HideInInspector] [SerializeReference]
        public NodeSlot parentNodeSlot;
        public object defaultValue;

        public object InvokeNode()
        {
            return parentNodeSlot?.node.Result();
        }
        
        public NodeSlot(Node node, string displayName, Type type)
        {
            this.node = node;
            name = displayName;
            SlotType = type;
        }

        public void AddParent(NodeSlot parent)
        {
            parentNodeSlot = parent;
        }
        
        public void Dispose()
        {
            Debug.Log("nodeSlot disposed.");
        }
    }
}