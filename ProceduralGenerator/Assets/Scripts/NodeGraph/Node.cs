using System;
using System.Collections.Generic;
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
            Selected,
        }

        public bool drawGizmos;
        [HideInInspector] public State state = State.Running;
        [SerializeReference] public Blackboard blackboard;
        [HideInInspector] public string guid = Guid.NewGuid().ToString();
        [HideInInspector] public Vector2 position;
        [HideInInspector] public GameObject parentObject;
        [HideInInspector] [SerializeReference]
        public List<NodeSlot> inputPorts = new();
        [HideInInspector] [SerializeReference]
        public List<NodeSlot> outputPorts = new();
        [HideInInspector] public bool dirty;
        private object savedResult;

        protected Node()
        {
            InitPorts();
        }

        public abstract object GetResult();

        public object Result()
        {
            savedResult = savedResult != null && !dirty ? savedResult : GetResult();
            dirty = false;

            if (savedResult is GameObject result && !result)
            {
                savedResult = GetResult();
            }
            
            return savedResult;
        }
        protected abstract void InitPorts();

        public void Abort() {
            NodeGraph.Traverse(this,  node=> {
                node.state = State.Running;
            });
        }

        public virtual void OnDrawGizmos() { }

        public NodeSlot AddNodeSlot(string name, string type, SlotDirection direction)
        {
            var slot = new NodeSlot(this, name, type);
            if (direction == SlotDirection.Input)
            {
                inputPorts.Add(slot);
            }
            else
            {
                outputPorts.Add(slot);
            }
            return slot;
        }
    }
}