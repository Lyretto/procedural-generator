using System;
using Lyred;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DefaultInputView : GraphElement, IDisposable
    {
        readonly CustomStyleProperty<Color> k_EdgeColorProperty = new ("--edge-color");

        Color m_EdgeColor = Color.red;

        public Color edgeColor => m_EdgeColor;
        public NodeSlot slot => m_Slot;

        NodeSlot m_Slot;
        Type m_SlotType;
        VisualElement m_Control;
        VisualElement m_Container;
        EdgeControl m_EdgeControl;

        public DefaultInputView(NodeSlot slot)
        {
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/DefaultInputView"));
            pickingMode = PickingMode.Ignore;
            ClearClassList();
            m_Slot = slot;
            m_SlotType = slot.SlotType;
            AddToClassList("type" + m_SlotType);

            m_EdgeControl = new EdgeControl
            {
                from = new Vector2(232f - 21f, 11.5f),
                to = new Vector2(232f, 11.5f),
                edgeWidth = 2,
                pickingMode = PickingMode.Ignore
            };
            Add(m_EdgeControl);

            m_Container = new VisualElement { name = "container" };
            {
                CreateControl();

                var slotElement = new VisualElement { name = "slot" };
                {
                    slotElement.Add(new VisualElement { name = "dot" });
                }
                m_Container.Add(slotElement);
            }
            Add(m_Container);

            m_Container.Add(new VisualElement() { name = "disabledOverlay", pickingMode = PickingMode.Ignore });
            RegisterCallback<CustomStyleResolvedEvent>(OnCustomStyleResolved);
        }

        void OnCustomStyleResolved(CustomStyleResolvedEvent e)
        {
            Color colorValue;

            if (e.customStyle.TryGetValue(k_EdgeColorProperty, out colorValue))
                m_EdgeColor = colorValue;

            m_EdgeControl.UpdateLayout();
            m_EdgeControl.inputColor = edgeColor;
            m_EdgeControl.outputColor = edgeColor;
        }

        public void UpdateSlot(NodeSlot newSlot)
        {
            m_Slot = newSlot;
            Recreate();
        }

        public void UpdateSlotType()
        {
            if (slot.SlotType != m_SlotType)
                Recreate();
        }

        private void Recreate()
        {
            RemoveFromClassList("type" + m_SlotType);
            m_SlotType = slot.SlotType;
            AddToClassList("type" + m_SlotType);
            if (m_Control != null)
            {
                if (m_Control is IDisposable disposable)
                    disposable.Dispose();
                m_Container.Remove(m_Control);
            }
            CreateControl();
        }

        private void CreateControl()
        {
            m_Container.visible = m_EdgeControl.visible = slot.parentNodeSlot == null;
        }

        public void Dispose()
        {
            if (m_Control is IDisposable disposable)
                disposable.Dispose();

            styleSheets.Clear();
            m_Control = null;
            m_EdgeControl = null;
            UnregisterCallback<CustomStyleResolvedEvent>(OnCustomStyleResolved);
        }
    
}
