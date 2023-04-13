using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Lyred {
    public class BlackboardGraphView : VisualElement {
        public new class UxmlFactory : UxmlFactory<BlackboardGraphView, UxmlTraits> { }

        public BlackboardGraphView() {

        }

        internal void Bind(SerializedNodeGraph serializer) {
            Clear();

            var blackboardProperty = serializer.Blackboard;
            blackboardProperty.isExpanded = true;
            var field = new PropertyField();
            field.BindProperty(blackboardProperty);
            Add(field);
        }
    }
}