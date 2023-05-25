using UnityEngine;

namespace Lyred
{
    public class PlaneNode : MeshNodeBase
    {
        public override object GetResult()
        {
            return GeneratePlane(GetVertCount(),GetSize());
        }

        private static Mesh GeneratePlane(int segments, float size)
            {
            var mesh = new Mesh();
            var vertices = new Vector3[(segments + 1) * (segments + 1)];
            var step = size / segments;

            for (int i = 0, z = 0; z <= segments; z++)
            {
                for (int x = 0; x <= segments; x++, i++)
                {
                    var xPos = x * step - size / 2f;
                    var zPos = z * step - size / 2f;
                    vertices[i] = new Vector3(xPos, 0f, zPos);
                }
            }

            // Assign the vertices to the mesh
            mesh.vertices = vertices;

            // Create the triangles for the plane mesh
            var triangles = new int[segments * segments * 6];

            for (int ti = 0, vi = 0, y = 0; y < segments; y++, vi++)
            {
                for (var x = 0; x < segments; x++, ti += 6, vi++)
                {
                    triangles[ti] = vi;
                    triangles[ti + 1] = vi + segments + 1;
                    triangles[ti + 2] = vi + 1;
                    triangles[ti + 3] = vi + 1;
                    triangles[ti + 4] = vi + segments + 1;
                    triangles[ti + 5] = vi + segments + 2;
                }
            }

            // Assign the triangles to the mesh
            mesh.triangles = triangles;

            // Create the UV coordinates for the plane mesh
            var uv = new Vector2[vertices.Length];

            for (var i = 0; i < uv.Length; i++)
            {
                uv[i] = new Vector2(vertices[i].x / size, vertices[i].z / size);
            }

            mesh.uv = uv;
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}