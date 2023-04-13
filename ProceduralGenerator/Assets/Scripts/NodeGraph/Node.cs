using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Lyred
{
    [Serializable]
    public abstract class Node
    {
        public enum State
        {
            Running,
            Failure,
            Success,
        }

        public State state = State.Running;
        public bool started = false;
        [HideInInspector] public Context context;
        [HideInInspector] public Blackboard blackboard;
        [HideInInspector] public string guid = Guid.NewGuid().ToString();
        [HideInInspector] public Vector2 position;
        public bool drawGizmos = false;
        [SerializeReference]
        public List<NodeSlot> inputPorts = new();
        [SerializeReference]
        public List<NodeSlot> outputPorts = new();

        protected Node()
        {
            InitPorts();
        }

        protected abstract void InitPorts();

        public State Update() {

            if (!started) {
                OnStart();
                started = true;
            }
            inputPorts.ForEach(parent => parent.ConnectedNode?.Update());
            
            state = OnUpdate();

            if (state == State.Running) return state;
            
            OnStop();
            started = false;

            return state;
        }

        public void Abort() {
            NodeGraph.Traverse(this, (node) => {
                node.started = false;
                node.state = State.Running;
                node.OnStop();
            });
        }

        public virtual void OnDrawGizmos() { }

        protected abstract void OnStart();
        protected abstract void OnStop();
        protected abstract State OnUpdate();
    }
    
    [Serializable]
    public struct NodePortInfo
    {
        [CanBeNull] public Node ConnectedNode;
        [CanBeNull] public Port ConnectedPort;
        public string ConnectedNodeGuid;
        public Type PortType;
        public string PortId;
        
        public NodePortInfo(Type portType, string portId)
        {
            PortType = portType;
            PortId = portId;
            ConnectedNode = null;
            ConnectedNodeGuid = null;
            ConnectedPort = null;
        }
    }
}