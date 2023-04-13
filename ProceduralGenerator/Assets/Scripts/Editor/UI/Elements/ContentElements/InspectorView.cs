using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Lyred
{
    public class InspectorView : VisualElement{
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }

        public InspectorView() {

        }

        internal void UpdateSelection(SerializedNodeGraph serializer, NodeView nodeView) {
            Clear();

            if (nodeView == null) {
                return;
            }

            var nodeProperty = SerializedNodeGraph.FindNode(serializer.Nodes, nodeView.node);
            if (nodeProperty == null) {
                return;
            }

            // Auto-expand the property
            nodeProperty.isExpanded = true;

            // Property field
            var field = new PropertyField
            {
                label = nodeProperty.managedReferenceValue.GetType().ToString()
            };
            field.BindProperty(nodeProperty);
            Add(field);
        }
    }
}