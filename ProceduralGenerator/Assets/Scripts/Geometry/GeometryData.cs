using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GeometryData
{
    public List<Vertex> Vertices = new();
    public List<Triangle> Triangles = new();
    public List<Edge> Edges = new();
    private Action<Color,bool> gizmosEvent;

    public void BindGizmos()
    {
        Vertices.ForEach(v => gizmosEvent += v.ShowGizmos);
        Edges.ForEach(v => gizmosEvent += v.ShowGizmos);
    }

    public void ShowGizmos(Color color, bool showNormals)
    {
        gizmosEvent.Invoke(color, showNormals);
    }

    public class Vertex
    {
        public Vector3 position;
        public Vector3 normal;
        public int index;

        public Vertex(Vector3 pos)
        {
            position = pos;
        }
        
        public void ShowGizmos(Color color, bool showNormals)
        {
            Gizmos.color = color;
            Gizmos.DrawSphere(position, 0.1f);
            if(showNormals) Gizmos.DrawLine(position, position + normal);
        }
    }

    public class Edge
    {
        public Vertex[] connection;

        public Edge(Vertex[] vertices)
        {
            connection = vertices;
        }

        public void Flip()
        {
            connection = connection.Reverse().ToArray();
        }
        
        public void ShowGizmos(Color color, bool showNormals)
        {
            Gizmos.color = color;
            Gizmos.DrawLine(connection[0].position, connection[1].position);
        }
    }

    public class Triangle
    {
        public Edge[] edges;

        public Triangle(Edge[] edges)
        {
            this.edges = edges;
        }

        public Vertex[] GetVertices()
        {
            return edges.Select(edge => edge.connection.First()).ToArray();
        }

        public void Flip()
        {
            foreach (var edge in edges)
            {
                edge.Flip();
            }
        }
    }
}