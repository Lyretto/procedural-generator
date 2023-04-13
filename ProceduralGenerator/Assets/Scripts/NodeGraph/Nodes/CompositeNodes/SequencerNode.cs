using System;

namespace Lyred
{
    public class SequencerNode : CompositeNode
    {
        protected override void InitPorts()
        {
            
        }

        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        
        }

        protected override State OnUpdate()
        {
            return State.Success;
        }
    }
}

