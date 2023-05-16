using UnityEngine;

namespace Lyred
{
    public abstract class ObjectNodeBase : Node
    {
        [SerializeReference]
        private NodeSlot objectSlot;

        protected override void InitPorts()
        {
            objectSlot = new NodeSlot(this, "GameObject", nameof(GameObject));
            outputPorts.Add(objectSlot);
        }
    }
}