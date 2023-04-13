using Lyred;
using UnityEngine;

public abstract class MeshNodeBase : Node
{
    private Node child;
    public abstract Mesh Result();
}
