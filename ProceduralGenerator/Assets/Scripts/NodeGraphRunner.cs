using UnityEngine;

namespace Lyred
{
    public class NodeGraphRunner : MonoBehaviour
    {
        public NodeGraph graph;

        private void Start() {
            
            //context = CreateBehaviourTreeContext();
            graph = graph.Clone();
            //graph.Bind(context);
            
            if(graph) graph.Generate(gameObject);
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
