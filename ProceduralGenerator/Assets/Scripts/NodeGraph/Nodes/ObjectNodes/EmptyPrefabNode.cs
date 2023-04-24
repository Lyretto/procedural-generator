using UnityEngine;

namespace Lyred
{
    public class EmptyPrefabNode : ObjectNodeBase
    {
        [SerializeReference]
        private NodeSlot pathSlot;
        protected override void InitPorts()
        {
            pathSlot = new NodeSlot(this, "Path", typeof(string));
            inputPorts.Add(pathSlot);
            base.InitPorts();
        }

        public override object GetResult()
        {
            var gameObject = new GameObject();
            //var prefab = PrefabUtility.SavePrefabAsset(gameObject);
            //Object.DestroyImmediate(gameObject);
            return gameObject;
        }
    }
}

