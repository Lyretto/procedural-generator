using Lyred;
using UnityEngine;

public abstract class MeshNodeBase : Node
{
    [SerializeReference]
    private NodeSlot sizeSlot;
    [SerializeReference]
    private NodeSlot vertsSlot;
    [SerializeReference]
    private NodeSlot meshSlot;
    
    protected override void InitPorts()
    {
        sizeSlot = new NodeSlot(this, "Size", typeof(float));
        vertsSlot = new NodeSlot(this, "Verts", typeof(int));
        meshSlot = new NodeSlot(this, "Mesh", typeof(Mesh)); ;
        inputPorts.Add(sizeSlot);
        inputPorts.Add(vertsSlot);
        outputPorts.Add(meshSlot);
    }

    protected float GetSize()
    {
        return (float)(sizeSlot.InvokeNode() ?? 1f);
    }
    
    protected int GetVertCount()
    {
        return (int)(vertsSlot.InvokeNode() ?? 1);
    }
}
