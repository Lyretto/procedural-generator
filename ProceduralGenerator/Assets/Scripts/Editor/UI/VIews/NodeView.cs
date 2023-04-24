using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Lyred {
    public class NodeView : UnityEditor.Experimental.GraphView.Node {
        public Action<NodeView> OnNodeSelected;
        private readonly SerializedNodeGraph serializer;
        public Node node;
        private string nodeTitle;

        public NodeView(SerializedNodeGraph tree, Node node)
        {
            this.node = node;
            base.title = node.GetType().Name;

            serializer = tree;
            viewDataKey = node.guid;

            style.left = node.position.x;
            style.top = node.position.y;

            CreateInputPorts();
            CreateOutputPorts();
        }

        private void CreateInputPorts()
        {
            node.inputPorts.ForEach(slot =>
            {
                var nodePort = new NodePort(Direction.Input, Port.Capacity.Single)
                {
                    portName = slot.name + " (" + (slot.SlotType?.Name ?? "") + ")" ,
                    viewDataKey = Guid.NewGuid().ToString(),
                };
                slot.guid = nodePort.viewDataKey;
                inputContainer.Add(nodePort);
            });
        }

        private void CreateDefaultNode(NodeSlot slot)
        {
            if (slot.defaultValue == null) return;
            
            var portContainer = new VisualElement();
            portContainer.style.flexDirection = FlexDirection.Row;
            var portInputView = new DefaultInputView(slot) { style = { position = Position.Absolute } };
            portContainer.Add(portInputView);
            inputContainer.Add(portContainer);
        }

        private void CreateOutputPorts() {
            
            node.outputPorts.ForEach(slot =>
            {
                var nodePort = new NodePort(Direction.Output, Port.Capacity.Single)
                {
                    portName = slot.name + " (" + (slot.SlotType?.Name ?? "") + ")" ,
                    viewDataKey = Guid.NewGuid().ToString(),
                };
                slot.guid = nodePort.viewDataKey;
                outputContainer.Add(nodePort);
            });
        }

        public override void SetPosition(Rect newPos) {
            base.SetPosition(newPos);

            var position = new Vector2(newPos.xMin, newPos.yMin);
            serializer.SetNodePosition(node, position);
        }

        public override void OnSelected() {
            base.OnSelected();
            OnNodeSelected?.Invoke(this);
        }

        private int SortByHorizontalPosition(Node left, Node right) {
            return left.position.x < right.position.x ? -1 : 1;
        }

        public void UpdateState() {
            RemoveFromClassList("failure");
            RemoveFromClassList("selected");

            if (!Application.isPlaying) return;

            switch (node.state)
            {
                case Node.State.Failure:
                    AddToClassList("failure");
                    break;
                case Node.State.Selected:
                    AddToClassList("selected");
                    break;
            }
        }
    }
}