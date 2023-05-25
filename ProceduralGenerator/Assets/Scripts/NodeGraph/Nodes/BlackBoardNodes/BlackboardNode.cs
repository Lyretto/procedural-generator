using System.Linq;
using UnityEngine;

namespace Lyred
{
    public class BlackboardNode : Node
    {
        [SerializeReference] private NodeSlot outputSlot;
        [SerializeReference] private string blackboardItemId;

        public override object GetResult()
        {
            return blackboard.items.Find(i => i.id == blackboardItemId).Value;
        }

        public void SetItem(string itemId)
        {
            blackboardItemId = itemId;
            var item = blackboard.items.FirstOrDefault(i => i.id == blackboardItemId); 
            outputSlot = new NodeSlot(this, item!.Id, item.type.ToString());
            outputPorts.Add(outputSlot);
        }
        
        protected override void InitPorts()
        {
            var item = blackboard?.items.FirstOrDefault(i => i.id == blackboardItemId);

            if (item == null) return;
            
            outputSlot = new NodeSlot(this, item.Id, item.type.ToString());
            outputPorts.Add(outputSlot);
        }
    }
}

