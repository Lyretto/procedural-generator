using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Lyred {
    public class SerializedNodeGraph 
    {
        private readonly SerializedObject serializedObject;
        public readonly NodeGraph graph;
        private const string sPropChildren = "children";
        private const string sPropPosition = "position";
        private const string sViewTransformPosition = "viewPosition";
        private const string sViewTransformScale = "viewScale";

        public SerializedProperty RootNode => serializedObject.FindProperty(nameof(RootNode).ToLower());
        public SerializedProperty Nodes => serializedObject.FindProperty(nameof(Nodes).ToLower());
        public SerializedProperty Blackboard => serializedObject.FindProperty(nameof(Blackboard).ToLower());
        
        public SerializedNodeGraph(NodeGraph graph)
        {
            serializedObject = new SerializedObject(graph);
            this.graph = graph;
        }

        private void Save() {

            serializedObject.ApplyModifiedProperties();
        }

        public void AddBlackboardItem(BlackboardItem item)
        {
            var newNode = AppendArrayElement(serializedObject.FindProperty("blackboard.items"));
            newNode.managedReferenceValue = item;
            Save();
        }
        
        public static SerializedProperty FindNode(SerializedProperty array, Node node) {
            for(var i = 0; i < array.arraySize; ++i) {
                var current = array.GetArrayElementAtIndex(i);
                var guid = current.FindPropertyRelative(nameof(node.guid).ToLower());
                if (guid.stringValue == node.guid) {
                    return current;
                }
            }
            return null;
        }

        public void SetViewTransform(Vector3 position, Vector3 scale) {
            serializedObject.FindProperty(sViewTransformPosition).vector3Value = position;
            serializedObject.FindProperty(sViewTransformScale).vector3Value = scale;
            Save();
        }

        public void SetNodePosition(Node node, Vector2 position) {
            var nodeProp = FindNode(Nodes, node);
            nodeProp.FindPropertyRelative(sPropPosition).vector2Value = position;
            Save();
        }

        private void DeleteNode(SerializedProperty array, Node node) {
            for (var i = 0; i < array.arraySize; ++i) {
                var current = array.GetArrayElementAtIndex(i);
                if (current.FindPropertyRelative(nameof(node.guid).ToLower()).stringValue != node.guid) continue;
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

            Save();

            return node;
        }

        public Node UpdateNode(Node node)
        {
            var nodeProperty = FindNode(Nodes, node);
            nodeProperty.managedReferenceValue = node;
            Save();
            return node;
        }

        public void SetRootNode(RootNode node) {
            RootNode.managedReferenceValue = node;
            Save();
        }

        public void DeleteNode(Node node) {
            var nodesProperty = Nodes;

            for(var i = 0; i < nodesProperty.arraySize; ++i) {
                var prop = nodesProperty.GetArrayElementAtIndex(i);
                var guid = prop.FindPropertyRelative(nameof(node.guid).ToLower()).stringValue;
                DeleteNode(Nodes, node);
                Save();
            }
        }

        public bool AddParent(Node child, Node parent, NodePort input, NodePort output)
        {
            var childProperty = FindNode(Nodes, child);
            var slotsProperty = childProperty.FindPropertyRelative(nameof(Node.inputPorts));
            if (slotsProperty == null) return false;

            for (var i = 0; i < slotsProperty.arraySize; ++i) {
                var current = slotsProperty.GetArrayElementAtIndex(i);
                if (current.FindPropertyRelative(nameof(child.guid).ToLower()).stringValue != input.viewDataKey) continue;
                if (parent.outputPorts.All(slot => slot.guid != output.viewDataKey)) continue;
                
                var parentSlot = parent.outputPorts.First(slot => slot.guid == output.viewDataKey);
                var childSLot = child.inputPorts.First(slot => slot.guid == input.viewDataKey);
                if(!childSLot.AddParent(parentSlot)) return false;
                current.FindPropertyRelative(nameof(childSLot.parentNodeSlot)).managedReferenceValue = parentSlot;
                return true;
            }
            Save();
            return false;
        }

        public void RemoveParent(Node child, NodePort input)
        {
            var childProperty = FindNode(Nodes, child);

            var slotsProperty = childProperty.FindPropertyRelative(sPropChildren);
            if (slotsProperty == null) return;
            
            for (var i = 0; i < slotsProperty.arraySize; ++i) {
                var current = slotsProperty.GetArrayElementAtIndex(i);
                if (current.FindPropertyRelative(nameof(child.guid).ToLower()).stringValue != input.viewDataKey) continue;
                current.FindPropertyRelative("parentNodeSlot").managedReferenceValue = null;
                return;
            }
            Save();
        }
    }
}
