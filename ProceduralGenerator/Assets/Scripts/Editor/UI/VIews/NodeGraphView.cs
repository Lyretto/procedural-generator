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

        private void OnViewTransformChanged(GraphView graphView)
        {
            if (serializer == null) return;
            
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
            
            serializer.graph.nodes.ForEach(CreateNodeView);

            serializer.graph.nodes.ForEach(node => {
                node.inputPorts.ForEach(inputPort =>
                {
                    if (inputPort.parentNodeSlot == null) return;
                    var port = FindPort(inputPort.guid);
                    
                    var connectedPort = FindPort(inputPort.parentNodeSlot.guid);

                    if (connectedPort == null) return;
                    var edge = port.ConnectTo(connectedPort);
                    if(edge != null) AddElement(edge);
                });
            });

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

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange) 
        {
            RemoveGraphElements(graphViewChange.elementsToRemove);
            graphViewChange.edgesToCreate = CreateEdges(graphViewChange.edgesToCreate);

            return graphViewChange;
        }

        private List<Edge> CreateEdges(List<Edge> edgesToCreate)
        {
            var createdEdges = new List<Edge>();
            edgesToCreate?.ForEach(edge => {
                var parentPort = edge.output as NodePort;
                var childPort = edge.input as NodePort;
                var isAdded = serializer.AddParent(((NodeView)childPort!.node).node, ((NodeView)parentPort!.node).node, childPort, parentPort);
                if (isAdded)
                {
                    createdEdges.Add(edge);
                }
            });
            return createdEdges;
        }

        private void RemoveGraphElements(List<GraphElement> elements)
        {
            elements?.ForEach(elem => {
                if (elem is NodeView nodeView) {
                    serializer.DeleteNode(nodeView.node);
                    OnNodeSelected(null);
                }

                if (elem is not Edge edge) return;
                var childPort = edge.input as NodePort;

                serializer.RemoveParent( ((NodeView)childPort!.node).node,  childPort);
            });
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt) 
        {
            if (evt.target is GraphElement view)
            {
                base.BuildContextualMenu(evt);
                return;
            }
            
            evt.menu.AppendSeparator();

            var nodePosition = this.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
            var nodeBaseTypes = TypeCache.GetTypesDerivedFrom<Node>();
            
            foreach (var nodeBaseType in nodeBaseTypes)
            {
                if (nodeBaseType.BaseType == typeof(Node)) continue;
                var nodeTypes = TypeCache.GetTypesDerivedFrom(nodeBaseType.BaseType);
                foreach (var nodeType in nodeTypes)
                {
                    evt.menu.AppendAction($"{nodeBaseType.BaseType!.Name}/{nodeType.Name}", (a) => CreateNode(nodeType, nodePosition));
                }
            }
        }
        
        private void CreateNode(Type type, Vector2 position) {
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
    }
}