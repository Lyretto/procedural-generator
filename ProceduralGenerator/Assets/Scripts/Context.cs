
using UnityEngine;

namespace Lyred
{
    public class Context
    {
        public GameObject gameObject;

        public static Context CreateFromGameObject(GameObject gameObject)
        {
            var context = new Context
            {
                gameObject = gameObject
            };

            return context;
        }
    }
}