using UnityEngine.UIElements;
using UnityEngine;

namespace Lyred {
    public class BlackboardGraphView : ScrollView {
        public new class UxmlFactory : UxmlFactory<BlackboardGraphView, UxmlTraits> { }
        

        public BlackboardGraphView()
        {
            styleSheets.Add(NodeGraphSettings.GetOrCreateSettings().blackBoardItemStyle);
        }

        private void CreateBlackboardItemView(BlackboardItem item, VisualTreeAsset treeAsset)
        {
            var newItem = new VisualElement
            {
                name = "BlackboardItemView",
            };
            treeAsset.CloneTree(newItem);
            
            var label = newItem.Q<Label>("itemName");
            label.text = item.Id;

            var typeLabel = newItem.Q<Label>("type-label");
            typeLabel.text = item.type.ToString();

            var knob = newItem.Q<VisualElement>("blackboardknob");
            knob.style.backgroundColor = Color.green;

            newItem.RegisterCallback<PointerDownEvent>(evt => OnItemDrag(evt, item, newItem));

            contentContainer.Add(newItem);
        }

        private void OnItemDrag(PointerDownEvent evt, BlackboardItem item, VisualElement view)
        {
            var newItem = new VisualElement
            {
                name = "BlackboardItemView",
                style = { position = Position.Absolute}
            };
            var blackboardView = NodeGraphSettings.GetOrCreateSettings().blackBoardItemXml;
            blackboardView.CloneTree(newItem);
            
            var label = newItem.Q<Label>("itemName");
            label.text = item.Id;

            var typeLabel = newItem.Q<Label>("type-label");
            typeLabel.text = item.type.ToString();

            var knob = newItem.Q<VisualElement>("blackboardknob");
            knob.style.backgroundColor = Color.green;

            newItem.transform.position = view.worldTransform.GetPosition() - new Vector3(0,view.worldBound.height,0);;

            rootElement.Add(newItem);
            var itemDragAndDropManipulator = new ItemDragAndDropManipulator(newItem, evt, item);
        }

        private void CreateBlackboardItems(Blackboard blackboard)
        {
            var blackboardView = NodeGraphSettings.GetOrCreateSettings().blackBoardItemXml;
            blackboard.items.ForEach(blackboardItem => CreateBlackboardItemView(blackboardItem, blackboardView));
        }

        private VisualElement rootElement;
        
        internal void Bind(Blackboard blackboard, VisualElement root)
        {
            rootElement = root;
            Clear();
            CreateBlackboardItems(blackboard);
        }
    }

    public class ItemDragAndDropManipulator : PointerManipulator
    {
        private BlackboardItem item;
        public ItemDragAndDropManipulator(VisualElement target, PointerDownEvent evt, BlackboardItem item)
        {
            this.target = target;
            root = target.parent;
            this.item = item;

            targetStartPosition = target.transform.position;
            pointerStartPosition = evt.position;
            target.CapturePointer(evt.pointerId);
            enabled = true;
        }
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerMoveEvent>(PointerMoveHandler);
            target.RegisterCallback<PointerUpEvent>(PointerUpHandler);
            target.RegisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerMoveEvent>(PointerMoveHandler);
            target.UnregisterCallback<PointerUpEvent>(PointerUpHandler);
            target.UnregisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler);
        }
        
        private Vector2 targetStartPosition { get; set; }

        private Vector3 pointerStartPosition { get; set; }

        private bool enabled { get; set; }

        private VisualElement root { get; }
        
        
        private void PointerMoveHandler(PointerMoveEvent evt)
        {
            if (!enabled || !target.HasPointerCapture(evt.pointerId)) return;

            var pointerDelta = evt.position - pointerStartPosition;
            target.transform.position = new Vector2(
                Mathf.Clamp(targetStartPosition.x + pointerDelta.x, 0, target.panel.visualTree.worldBound.width),
                Mathf.Clamp(targetStartPosition.y + pointerDelta.y, 0, target.panel.visualTree.worldBound.height));
        }

        private void PointerUpHandler(PointerUpEvent evt)
        {
            if (enabled && target.HasPointerCapture(evt.pointerId))
            {
                var graphContainer = root.panel.visualTree.Q<NodeGraphView>();

                if (IsInside(graphContainer.worldBound, target.worldBound))
                {
                    graphContainer.CreateBlackboardNode(item, target.worldBound.position);
                }
                
                target.ReleasePointer(evt.pointerId);
                root.Remove(target);
            }
        }

        private bool IsInside(Rect box, Rect other)
        {
            return box.Contains(other.max - other.size/2) && box.Contains(other.min + other.size/2);
        }
        
        private void PointerCaptureOutHandler(PointerCaptureOutEvent evt)
        {
            if (enabled)
            {
                enabled = false;
                root.Remove(target);
            }
        }
    }
}