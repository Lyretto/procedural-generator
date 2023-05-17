using System;
using Lyred;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;
using Object = UnityEngine.Object;

[CustomEditor(typeof(NodeGraphRunner))]
public class NodeGraphRunnerInspector : Editor
{
    private NodeGraphRunner runner;
    private void OnEnable()
    {
        runner = (NodeGraphRunner) target;
    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Bake"))
        {
            runner.graph.Generate(runner.gameObject);
        }
        
        foreach (var blackboardItem in runner.graph.blackboard.items)
        {
            GUILayout.BeginHorizontal("box");
            GUILayout.Label(blackboardItem.Id);
            switch (blackboardItem.Type)
            {
                case "int":
                    blackboardItem.Value = EditorGUILayout.IntField(blackboardItem.Value is int intValue ? intValue : 0);
                    break;
                case "GameObject":
                    blackboardItem.Value = EditorGUILayout.ObjectField(blackboardItem.Value as GameObject, typeof(object), true);
                    break;
                case "Mesh":
                    blackboardItem.Value = EditorGUILayout.ObjectField(blackboardItem.Value as Mesh, typeof(Mesh), true);
                    break;
                case "Spline":
                    blackboardItem.Value = EditorGUILayout.ObjectField(blackboardItem.Value as SplineContainer, typeof(SplineContainer), true);
                    break;
                case "Color":
                    blackboardItem.Value = EditorGUILayout.ColorField(blackboardItem.Value is Color color ? color : default);
                    break;
                case "float":
                    blackboardItem.Value =
                        EditorGUILayout.FloatField(blackboardItem.Value is float floatValue ? floatValue : 0);
                    break;
            }
            
            GUILayout.EndHorizontal();
        }

        base.OnInspectorGUI();
    }
    
}
