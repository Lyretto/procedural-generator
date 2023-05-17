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
            if (GUILayout.Button("Generate"))
            {
                runner.graph.Generate(runner.gameObject);
            }

            BlackBoardItems();
            
            base.OnInspectorGUI();
        }

        private void BlackBoardItems()
        {
            EditorGUILayout.Separator();
            GUILayout.Label("Blackboard");
            foreach (var blackboardItem in runner.graph.blackboard.items)
            {
                GUILayout.BeginHorizontal("box");
                GUILayout.Label(blackboardItem.Id);
                blackboardItem.Value = blackboardItem.Type switch
                {
                    "int" => EditorGUILayout.IntField(blackboardItem.GetValue() is int intValue ? intValue : 0),
                    "bool" => EditorGUILayout.Toggle(blackboardItem.GetValue() is bool),
                    "GameObject" => EditorGUILayout.ObjectField(blackboardItem.GetValue() as GameObject, typeof(object),
                        true),
                    "Mesh" => EditorGUILayout.ObjectField(blackboardItem.GetValue() as Mesh, typeof(Mesh), true),
                    "Spline" => EditorGUILayout.ObjectField(blackboardItem.GetValue() as SplineContainer,
                        typeof(SplineContainer), true),
                    "Color" => EditorGUILayout.ColorField(blackboardItem.GetValue() is Color color ? color : default),
                    "float" => EditorGUILayout.FloatField(blackboardItem.GetValue() is float floatValue ? floatValue : 0),
                    _ => blackboardItem.GetValue()
                };
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.Separator();
        }
    }
}