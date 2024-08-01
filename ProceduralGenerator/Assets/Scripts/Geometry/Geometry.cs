using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Geometry
{
    static readonly Vector3[] SquareDirectionVector = { Vector3.right, Vector3.forward };

    public static GeometryData CreateLine(int segments, float length, Vector3 startPos, Vector3 direction)
    {
        segments++;
        var geometry = new GeometryData();
        var step = length / segments;

        for (var i = 0; i < segments; i++)
        {
            var edgeVerts = geometry.Vertices.Last();
            
            geometry.Vertices.Add(new GeometryData.Vertex( startPos + direction * (step * i)));

            if (edgeVerts != null)
            {
                geometry.Edges.Add(new GeometryData.Edge( new []{edgeVerts, geometry.Vertices.Last()}));
            }
        }
        return geometry;
    }

    public static GeometryData CreateGrid(int segments, float size)
    {
        var geometry = new GeometryData();
        
        var line = CreateLine(segments, size, Vector3.zero, SquareDirectionVector[0]).Vertices;
        
        foreach (var vertex in line)
        {
            geometry.Vertices.AddRange(CreateLine(segments, size, vertex.position,SquareDirectionVector[1]).Vertices);
        }
        
        //geometry.Vertices = geometry.Vertices.Distinct().ToList();
        
        return geometry;
    }

    public static GeometryData Merge(params GeometryData[] geoToMerge)
    {
        var geo = new GeometryData();
        return geo;
    }
    

    public static Mesh GeometryToMesh(GeometryData geometryData)
    {
        var mesh = new Mesh();
        var vertices = new List<GeometryData.Vertex>();
        var triangles = new List<int>();
        
        for (var i = 0; i < geometryData.Vertices.Count; i++)
        {
            vertices.Add(geometryData.Vertices[i]);
            vertices[i].index = i;
        }
        
        foreach (var triangle in geometryData.Triangles)
        {
            triangles.AddRange(triangle.GetVertices().Select(t => t.index));
        }

        mesh.vertices = geometryData.Vertices.Select(v => v.position).ToArray();
        mesh.triangles = triangles.ToArray();

        return mesh;
    }
}
