using System;
using System.Linq;
using UnityEngine;

namespace Lyred
{
    public class EmptyGameObject : ObjectNodeBase
    {
        [SerializeReference] private NodeSlot pathSlot;
        [SerializeReference] private NodeSlot parentSlot;
        protected override void InitPorts()
        {
            pathSlot = AddNodeSlot("Path", "string", SlotDirection.Input);
            parentSlot = AddNodeSlot("Parent", nameof(GameObject), SlotDirection.Input);
            base.InitPorts();
        }

        public override object GetResult()
        {
            var gameObject = new GameObject();
            var parent = ((GameObject) parentSlot?.InvokeNode())?.transform! ?? parentObject.transform;
            gameObject.transform.SetParent(parent, false);
            gameObject.hideFlags = HideFlags.DontSaveInEditor & HideFlags.DontSaveInBuild;
            //var prefab = PrefabUtility.SavePrefabAsset(gameObject);
            //Object.DestroyImmediate(gameObject);
            return gameObject;
        }
    }
}

