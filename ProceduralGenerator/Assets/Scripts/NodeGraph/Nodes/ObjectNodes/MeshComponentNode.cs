using System.Linq;
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
        gameObjectSlot = new NodeSlot(this, "Prefab", typeof(GameObject));
        meshSlot = new NodeSlot(this, "Mesh", typeof(Mesh));
        inputPorts.Add(gameObjectSlot);
        inputPorts.Add(meshSlot);
        base.InitPorts();
    }

    public override object GetResult()
    {
        var gameObject = (GameObject) gameObjectSlot.InvokeNode();
        gameObject.AddComponent<MeshRenderer>();
        var meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = (Mesh)meshSlot.InvokeNode();
        
        return gameObject;
    }
}
