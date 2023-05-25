using Lyred;
using UnityEngine;

public class MeshComponentNode : ObjectNodeBase
{
    [SerializeReference]
    private NodeSlot gameObjectSlot;
    [SerializeReference]
    private NodeSlot meshSlot;
    protected override void InitPorts()
    {
        gameObjectSlot = new NodeSlot(this, "Prefab", nameof(GameObject));
        meshSlot = new NodeSlot(this, "Mesh", nameof(Mesh));
        inputPorts.Add(gameObjectSlot);
        inputPorts.Add(meshSlot);
        base.InitPorts();
    }

    public override object GetResult()
    {
        var gameObject = (GameObject) gameObjectSlot.InvokeNode();

        if (!gameObject) return null;
        
        if(!gameObject.TryGetComponent<MeshRenderer>(out _)) gameObject.AddComponent<MeshRenderer>();
        
        if (!gameObject.TryGetComponent(out MeshFilter meshFilter))
        {
           meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        meshFilter.mesh = (Mesh)meshSlot.InvokeNode();
        
        return gameObject;
    }
}
