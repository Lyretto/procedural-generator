using UnityEngine;

namespace Lyred
{
    public class LineNode : GeometryNodeBase
    {
        private GeometryData line;
        public override object GetResult()
        {
            return GetLine();
        }

        private GeometryData GetLine()
        {
            if (line != null && !dirty) return line;
            
            line = Geometry.CreateLine(GetVertCount(), GetSize(), Vector3.zero, Vector3.right);

            return line;
        }

        public override void OnDrawGizmos()
        {
            GetLine().ShowGizmos(Color.blue, false);
        }
    }
}