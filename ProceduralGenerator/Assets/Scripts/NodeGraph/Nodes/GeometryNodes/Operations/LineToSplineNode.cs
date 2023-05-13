using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

namespace Lyred
{
    public class LineToSplineNode : Node
    {
        [SerializeReference] private NodeSlot lineSlot;
        [SerializeReference] private NodeSlot splineSlot;
        
        private GeometryData line;

        public override object GetResult()
        {
           line = (GeometryData) lineSlot.InvokeNode();
           var knots = line.Vertices.Select(vertex => new BezierKnot(vertex.position));
           return new Spline(knots);
        }

        protected override void InitPorts()
        {
            lineSlot = new NodeSlot(this, "Line", typeof(GeometryData).ToString());
            splineSlot = new NodeSlot(this, "Spline", typeof(Spline).ToString());
            inputPorts.Add(lineSlot);
            outputPorts.Add(splineSlot);
        }
    } 
}

