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
            lineSlot = new NodeSlot(this, "Line", nameof(GeometryData));
            splineSlot = new NodeSlot(this, "Spline", nameof(Spline));
            inputPorts.Add(lineSlot);
            outputPorts.Add(splineSlot);
        }
    } 
}

