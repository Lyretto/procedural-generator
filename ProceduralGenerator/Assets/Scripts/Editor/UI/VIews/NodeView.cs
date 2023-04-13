using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

namespace Lyred {

    public class NodeView : UnityEditor.Experimental.GraphView.Node {
        public Action<NodeView> OnNodeSelected;
        private SerializedNodeGraph serializer;
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
            node.inputPorts.ForEach(port =>
            {
                var nodePort = new NodePort(Direction.Input, Port.Capacity.Single)
                {
                    portName = port.PortType?.Name ?? "",
                };
                port.ConnectedPort = nodePort;
                inputContainer.Add(nodePort);
            });
        }

        private void CreateOutputPorts() {
            
            node.outputPorts.ForEach(port =>
            {
                var nodePort = new NodePort(Direction.Output, Port.Capacity.Single)
                {
                    portName = port.PortType.Name,
                };
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

        public void SortChildren() {
            if (node is CompositeNode composite) {
                composite.children.Sort(SortByHorizontalPosition);
            }
        }

        private int SortByHorizontalPosition(Node left, Node right) {
            return left.position.x < right.position.x ? -1 : 1;
        }

        public void UpdateState() {

            RemoveFromClassList("running");
            RemoveFromClassList("failure");
            RemoveFromClassList("success");

            if (!Application.isPlaying) return;
            
            switch (node.state) {
                case Node.State.Running:
                    if (node.started) {
                        AddToClassList("running");
                    }
                    break;
                case Node.State.Failure:
                    AddToClassList("failure");
                    break;
                case Node.State.Success:
                    AddToClassList("select");
                    break;
            }
        }
    }
}