using Unity.VisualScripting;
using UnityEngine;

namespace Lyred
{
    public class RepeatNode : DecoratorNode
    {
        protected override void InitPorts()
        {
            inputPorts.Add(new NodePortInfo(typeof(Mesh), "CubeMesh"));
            inputPorts.Add(new NodePortInfo(typeof(Vector3), "Position"));
            outputPorts.Add(new NodePortInfo(typeof(Object), "Object"));
        }
        
        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            child.Update();
            return State.Running;
        }
    }
}
