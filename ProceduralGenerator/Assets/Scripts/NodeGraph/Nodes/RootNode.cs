using UnityEngine;

namespace Lyred
{
    [System.Serializable]

    public class RootNode : Node
    {
        public override object GetResult()
        {
           // inputPorts.ForEach(slots => slots.parentNodeSlot?.node.Result());
            return null;
        }

        protected override void InitPorts()
        {
            inputPorts.Add(new NodeSlot(this,"Object",nameof(GameObject)));
        }
    }
}
