using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine;

namespace Lyred {
    public class BlackboardGraphView : VisualElement {
        public new class UxmlFactory : UxmlFactory<BlackboardGraphView, UxmlTraits> { }

        public BlackboardGraphView()
        {
            var name = "Item";
            var color = Color.green;
            var item = new VisualElement();

            var labelContainer = new VisualElement();
            var knobContainer = new VisualElement();
            var knob = new VisualElement
            {
                style =
                {
                    backgroundColor = new StyleColor(color),
                    width = 10,
                    height = 10,
                    alignSelf = Align.Center,
                    flexGrow = 0,
                    borderBottomLeftRadius = 5,
                    borderBottomRightRadius = 5,
                    borderTopLeftRadius = 5,
                    borderTopRightRadius = 5
                }
            };
            knobContainer.Add(knob);
            item.Add(knobContainer);
            item.Add(labelContainer);
            labelContainer.Add(new Label(name));

            Add(item);
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