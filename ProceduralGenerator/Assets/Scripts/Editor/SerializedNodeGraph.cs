using UnityEngine;
using UnityEditor;

namespace Lyred {
    public class SerializedNodeGraph 
    {
        private readonly SerializedObject serializedObject;
        public readonly NodeGraph graph;
        private const string sPropRootNode = "rootNode";
        private const string sPropNodes = "nodes";
        private const string sPropBlackboard = "blackboard";
        private const string sPropGuid = "guid";
        private const string sPropChild = "child";
        private const string sPropChildren = "children";
        private const string sPropPosition = "position";
        private const string sViewTransformPosition = "viewPosition";
        private const string sViewTransformScale = "viewScale";

        public SerializedProperty RootNode => serializedObject.FindProperty(sPropRootNode);
        public SerializedProperty Nodes => serializedObject.FindProperty(sPropNodes);
        public SerializedProperty Blackboard => serializedObject.FindProperty(sPropBlackboard);
        
        public SerializedNodeGraph(NodeGraph graph)
        {
            serializedObject = new SerializedObject(graph);
            this.graph = graph;
        }

        public void Save() {
            serializedObject.ApplyModifiedProperties();
        }

        public static SerializedProperty FindNode(SerializedProperty array, Node node) {
            for(var i = 0; i < array.arraySize; ++i) {
                var current = array.GetArrayElementAtIndex(i);
                var guid = current.FindPropertyRelative(sPropGuid);
                if (guid.stringValue == node.guid) {
                    return current;
                }
            }
            return null;
        }

        public void SetViewTransform(Vector3 position, Vector3 scale) {
            serializedObject.FindProperty(sViewTransformPosition).vector3Value = position;
            serializedObject.FindProperty(sViewTransformScale).vector3Value = scale;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        public void SetNodePosition(Node node, Vector2 position) {
            var nodeProp = FindNode(Nodes, node);
            nodeProp.FindPropertyRelative(sPropPosition).vector2Value = position;
            serializedObject.ApplyModifiedProperties();
        }

        private void DeleteNode(SerializedProperty array, Node node) {
            for (var i = 0; i < array.arraySize; ++i) {
                var current = array.GetArrayElementAtIndex(i);
                if (current.FindPropertyRelative(sPropGuid).stringValue != node.guid) continue;
                array.DeleteArrayElementAtIndex(i);
                return;
            }
        }

        private static Node CreateNodeInstance(System.Type type) {
            var node = System.Activator.CreateInstance(type) as Node;
            node!.guid = GUID.Generate().ToString();
            return node;
        }

        private static SerializedProperty AppendArrayElement(SerializedProperty arrayProperty) {
            arrayProperty.InsertArrayElementAtIndex(arrayProperty.arraySize);
            return arrayProperty.GetArrayElementAtIndex(arrayProperty.arraySize - 1);
        }

        public Node CreateNode(System.Type type, Vector2 position) {

            var node = CreateNodeInstance(type);
            node.position = position;

            var newNode = AppendArrayElement(Nodes);
            newNode.managedReferenceValue = node;

            serializedObject.ApplyModifiedProperties();

            return node;
        }

        public void SetRootNode(RootNode node) {
            RootNode.managedReferenceValue = node;
            serializedObject.ApplyModifiedProperties();
        }

        public void DeleteNode(Node node) {

            var nodesProperty = Nodes;

            for(var i = 0; i < nodesProperty.arraySize; ++i) {
                var prop = nodesProperty.GetArrayElementAtIndex(i);
                var guid = prop.FindPropertyRelative(sPropGuid).stringValue;
                DeleteNode(Nodes, node);
                serializedObject.ApplyModifiedProperties();
            }
        }
        public void AddChild(Node parent, Node child) {
            
            var parentProperty = FindNode(Nodes, parent);

            // RootNode, Decorator node
            var childProperty = parentProperty.FindPropertyRelative(sPropChild);
            if (childProperty != null) {
                childProperty.managedReferenceValue = child;
                serializedObject.ApplyModifiedProperties();
                return;
            }

            // Composite nodes
            var childrenProperty = parentProperty.FindPropertyRelative(sPropChildren);
            if (childrenProperty != null) {
                var newChild = AppendArrayElement(childrenProperty);
                newChild.managedReferenceValue = child;
                serializedObject.ApplyModifiedProperties();
                return;
            }
        }
        
        public void AddParent(Node child, Node parent, NodePort input, NodePort output)
        {
            var childProperty = FindNode(Nodes, child);
            
            var parentProperty = childProperty.FindPropertyRelative(nameof(Node.inputPorts));
            if (parentProperty == null) return;
            
            
            var newParent = AppendArrayElement(parentProperty);
            var port = (NodeSlot) newParent.boxedValue;

            newParent.managedReferenceValue = port;
            serializedObject.ApplyModifiedProperties();
        }

        public void RemoveParent(Node child, Node parent)
        {
            var childProperty = FindNode(Nodes, child);

            // Composite nodes
            var parentsProperty = childProperty.FindPropertyRelative(sPropChildren);
            if (parentsProperty == null) return;
            
            DeleteNode(parentsProperty, parent);
            serializedObject.ApplyModifiedProperties();
        }

        public void RemoveChild(Node parent, Node child) {
            var parentProperty = FindNode(Nodes, parent);

            // RootNode, Decorator node
            var childProperty = parentProperty.FindPropertyRelative(sPropChild);
            if (childProperty != null) {
                childProperty.managedReferenceValue = null;
                serializedObject.ApplyModifiedProperties();
                return;
            }

            // Composite nodes
            var childrenProperty = parentProperty.FindPropertyRelative(sPropChildren);
            if (childrenProperty != null) {
                DeleteNode(childrenProperty, child);
                serializedObject.ApplyModifiedProperties();
                return;
            }
        }
    }
}
