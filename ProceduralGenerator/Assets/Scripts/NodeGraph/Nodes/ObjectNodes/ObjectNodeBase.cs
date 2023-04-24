using UnityEngine;

namespace Lyred
{
    public abstract class ObjectNodeBase : Node
    {
        [SerializeReference]
        private NodeSlot objectSlot;

        protected override void InitPorts()
        {
            objectSlot = new NodeSlot(this, "GameObject", typeof(GameObject));
            outputPorts.Add(objectSlot);
        }
    }
}