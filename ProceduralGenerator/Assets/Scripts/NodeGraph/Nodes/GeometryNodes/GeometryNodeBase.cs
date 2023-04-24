using Lyred;
using UnityEngine;

public abstract class GeometryNodeBase : Node
{
    [SerializeReference] private NodeSlot sizeSlot;
    [SerializeReference] private NodeSlot vertsSlot;
    [SerializeReference] private NodeSlot geometrySlot;
    
    protected override void InitPorts()
    {
        sizeSlot = new NodeSlot(this, "Size", typeof(float));
        vertsSlot = new NodeSlot(this, "Verts", typeof(int));
        geometrySlot = new NodeSlot(this, "Geometry", typeof(GeometryData)); ;
        inputPorts.Add(sizeSlot);
        inputPorts.Add(vertsSlot);
        outputPorts.Add(geometrySlot);
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
