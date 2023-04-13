using UnityEngine;

namespace Lyred
{
    [System.Serializable]

    public class RootNode : Node
    {
        [SerializeReference] [HideInInspector] public Node parent;


        protected override void InitPorts()
        {
            inputPorts.Add(new NodePortInfo(typeof(Object), "Object"));
        }

        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            return parent.Update();
        }
    }
}
