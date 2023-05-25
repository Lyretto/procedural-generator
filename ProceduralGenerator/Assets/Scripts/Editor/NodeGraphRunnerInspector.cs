using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

namespace Lyred
{
    [CustomEditor(typeof(NodeGraphRunner))]
    public class NodeGraphRunnerInspector : Editor
    {
        private NodeGraphRunner runner;

        private void OnEnable()
        {
            runner = (NodeGraphRunner)target;
        }

        public override void OnInspectorGUI()
        {
            if (runner && runner.graph)
            {
                
                if (GUILayout.Button("Generate"))
                {
                    runner.graph.Generate(runner.gameObject);
                }

                BlackBoardItems();
            }
            base.OnInspectorGUI();
        }

        private void BlackBoardItems()
        {
            if (!runner || !runner.graph) return;
        
            EditorGUILayout.Separator();
            GUILayout.Label("Blackboard");
            foreach (var blackboardItem in runner.graph.blackboard.items)
            {
                GUILayout.BeginHorizontal("box");
                GUILayout.Label(blackboardItem.Id);
                var newValue = blackboardItem.type switch
                {
                    ItemType.Int => EditorGUILayout.IntField(blackboardItem.GetValue() is int intValue ? intValue : 0),
                    ItemType.Bool => EditorGUILayout.Toggle(blackboardItem.GetValue() is bool),
                    ItemType.GameObject => EditorGUILayout.ObjectField(blackboardItem.GetValue() as GameObject, typeof(object),
                        true),
                    ItemType.Mesh => EditorGUILayout.ObjectField(blackboardItem.GetValue() as Mesh, typeof(Mesh), true),
                    ItemType.Spline => EditorGUILayout.ObjectField(blackboardItem.GetValue() as SplineContainer,
                        typeof(SplineContainer), true),
                    ItemType.Color => EditorGUILayout.ColorField(blackboardItem.GetValue() is Color color ? color : default),
                    ItemType.Float => EditorGUILayout.FloatField(blackboardItem.GetValue() is float floatValue ? floatValue : 0),
                    _ => blackboardItem.GetValue()
                };

                if (!Equals(newValue, blackboardItem.Value)) blackboardItem.Value = newValue;
                GUILayout.EndHorizontal();
                
            }
            EditorGUILayout.Separator();
        }
    }
}