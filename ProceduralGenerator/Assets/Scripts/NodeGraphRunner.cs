using UnityEngine;

namespace Lyred
{
    public class NodeGraphRunner : MonoBehaviour
    {
        public NodeGraph graph;
        // Storage container object to hold game object subsystems
        Context context;

        // Start is called before the first frame update
        private void Start() {
            context = CreateBehaviourTreeContext();
            graph = graph.Clone();
            graph.Bind(context);
        }

        // Update is called once per frame
        private void Update() {
            if (graph) {
                graph.Update();
            }
        }

        private Context CreateBehaviourTreeContext() {
            return Context.CreateFromGameObject(gameObject);
        }

        private void OnDrawGizmosSelected() {
            if (!graph) {
                return;
            }

            NodeGraph.Traverse(graph.rootNode, (n) => {
                if (n.drawGizmos) {
                    n.OnDrawGizmos();
                }
            });
        }
    }
}
