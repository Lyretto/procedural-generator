using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using System.Linq;

namespace Lyred {
    public class NodeGraphView : GraphView {
        public new class UxmlFactory : UxmlFactory<NodeGraphView, UxmlTraits> { }

        public Action<NodeView> OnNodeSelected;

        SerializedNodeGraph serializer;
        NodeGraphSettings settings;
        
        public NodeGraphView() {
            settings = NodeGraphSettings.GetOrCreateSettings();

            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var styleSheet = settings.behaviourTreeStyle;
            styleSheets.Add(styleSheet);

            viewTransformChanged += OnViewTransformChanged;
        }

        private void OnViewTransformChanged(GraphView graphView) {
            var position = contentViewContainer.transform.position;
            var transformScale = contentViewContainer.transform.scale;
            serializer.SetViewTransform(position, transformScale);
        }

        private NodeView FindNodeView(Node node) {
            return GetNodeByGuid(node.guid) as NodeView;
        }

        public void ClearView() {
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements.ToList());
            graphViewChanged += OnGraphViewChanged;
        }

        public void PopulateView(SerializedNodeGraph tree) {
            serializer = tree;
            
            ClearView();

            Debug.Assert(serializer.graph.rootNode != null);

            // Creates node view
            serializer.graph.nodes.ForEach(CreateNodeView);

            // Create edges
            serializer.graph.nodes.ForEach(node => {
                node.inputPorts.ForEach(inputPort =>
                {
                    var port = FindPort(inputPort.guid);
                    var connectedPort = FindPort(inputPort.parentGuid);
                    var edge = port.ConnectTo(connectedPort);

                    // var parentView = FindNodeView(inputPort.ConnectedNode);
                    //var edge = ;inputPort.?.ConnectTo(inputPort.ConnectedNode.outputPorts.First(outputPort => outputPort.PortType == inputPort.PortType).ConnectedPort);
                    if(edge != null) AddElement(edge);
                });
            });

            // Set view
            contentViewContainer.transform.position = serializer.graph.viewPosition;
            contentViewContainer.transform.scale = serializer.graph.viewScale;
        }
        
        private NodePort FindPort(string portGuid)
        {
                return (NodePort) GetPortByGuid(portGuid);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) {
            return ports.ToList().Where(endPort =>
            endPort.direction != startPort.direction &&
            endPort.node != startPort.node).ToList();
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange) {
            graphViewChange.elementsToRemove?.ForEach(elem => {
                if (elem is NodeView nodeView) {
                    serializer.DeleteNode(nodeView.node);
                    OnNodeSelected(null);
                }

                if (elem is not Edge edge) return;
                var parentView = edge.output.node as NodeView;
                var childView = edge.input.node as NodeView;
                //serializer.RemoveChild(parentView!.node, childView!.node);
                serializer.RemoveParent( childView!.node,parentView!.node);
            });

            graphViewChange.edgesToCreate?.ForEach(edge => {
                var parentView = edge.output as NodePort;
                var childView = edge.input as NodePort;
                //serializer.AddChild(parentView!.node, childView!.node);
                serializer.AddParent( childView,parentView);
            });

            nodes.ForEach((n) => {
                var view = n as NodeView;
                view!.SortChildren();
            });

            return graphViewChange;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt) {

            //base.BuildContextualMenu(evt);

            evt.menu.AppendSeparator();

            var nodePosition = this.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
            {

                var types = TypeCache.GetTypesDerivedFrom<ActionNode>();
                foreach (var type in types) {
                    evt.menu.AppendAction($"[Action]/{type.Name}", (a) => CreateNode(type, nodePosition));
                }
            }

            {
                var types = TypeCache.GetTypesDerivedFrom<CompositeNode>();
                foreach (var type in types) {
                    evt.menu.AppendAction($"[Composite]/{type.Name}", (a) => CreateNode(type, nodePosition));
                }
            }

            {
                var types = TypeCache.GetTypesDerivedFrom<DecoratorNode>();
                foreach (var type in types) {
                    evt.menu.AppendAction($"[Decorator]/{type.Name}", (a) => CreateNode(type, nodePosition));
                }
            }
        }
        
        private void CreateNode(System.Type type, Vector2 position) {
            var node = serializer.CreateNode(type, position);
            CreateNodeView(node);
        }

        private void CreateNodeView(Node node) {
            var nodeView = new NodeView(serializer, node)
            {
                OnNodeSelected = OnNodeSelected
            };
            AddElement(nodeView);
        }

        public void UpdateNodeStates() {
            nodes.ForEach(n => {
                var view = n as NodeView;
                view?.UpdateState();
            });
        }
    }
}