using System;
using Lyred;
using UnityEditor;
using UnityEngine;

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
        base.OnInspectorGUI();
    }
    
}
