using System;
using UnityEngine;


namespace Lyred
{
    [Serializable]
    public class NodeSlot : IDisposable
    {
        [SerializeReference]
        public string guid;
        [SerializeReference]
        public string parentGuid;
        private string name;
        private Type slotType;



        public NodeSlot(string portGuid, string displayName, Type type)
        {
            guid = portGuid;
            name = displayName;
            slotType = type;
        }
        
        public void Dispose()
        {
            Debug.Log("nodeSlot disposed.");
        }
    }
}