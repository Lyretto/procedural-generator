
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class GroundGenerator : MonoBehaviour
{
    public List<Vector2Int> tilePositions = new List<Vector2Int>();
    public float tileSize = 1.0f;
    public Material grass;
    public Texture2D grassTexture;
    public Texture2D coastTexture;
    public Vector2 grassTiling = new Vector2(1, 1);
    public Vector2 coastTiling = new Vector2(1, 1);
    private int _segments = 4;
    private float offsetRange = 0.1f;

    private GameObject ground;
    public List<Vector3> closedPositions = new ();

    public Dictionary<Color,List<Vector3>> debugPoints = new ();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _segments--;
            GenerateGroundWithProBuilder();
        }
        
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _segments++;
            GenerateGroundWithProBuilder();
        }
    }

    void Start()
    {
        tilePositions = new List<Vector2Int>
        {
            new Vector2Int(0, 0),
            new Vector2Int(1, 0),
            new Vector2Int(2, 0),
            new Vector2Int(0, 1),
            //new Vector2Int(1, 1),
            new Vector2Int(2, 1),
            new Vector2Int(0, 2),
            new Vector2Int(1, 2),
            new Vector2Int(2, 2),
            new Vector2Int(0, 3),
            new Vector2Int(1, 3),
        };

        GenerateGroundWithProBuilder();
    }

    void GenerateGroundWithProBuilder()
    {
        if (ground)
        {
            Destroy(ground);
        }
        
        ground = new GameObject("Ground");
        var vertices = new Dictionary<int, Vector3>();   
        
        var faces = new List<Face>();
        var pbMesh = ground.AddComponent<ProBuilderMesh>();
        var vertexIndex = 0;

        foreach (var pos in tilePositions)
        {
            var v0 = vertexIndex;
            var v1 = vertexIndex + 1;
            var v2 = vertexIndex + 2;
            var v3 = vertexIndex + 3;
            vertices.Add(v0, new Vector3(pos.x,0,pos.y));
            vertices.Add(v1, new Vector3(pos.x + 1,0,pos.y));
            vertices.Add(v2, new Vector3(pos.x,0,pos.y + 1));
            vertices.Add(v3, new Vector3(pos.x + 1,0,pos.y + 1));
            
           var newFace = new Face(new [] { v0, v3, v1, v0, v2, v3 });
           
           newFace.ToQuad();
           faces.Add(newFace); 
           vertexIndex += 4;
        }
        
        pbMesh.RebuildWithPositionsAndFaces(vertices.Select(v => v.Value), faces);
        
        var strength = 0.1f;
        Dictionary<Edge, Vector3> lastExtruded = null;
        
        for (var i = 0; i < _segments; i++)
        {
            pbMesh.WeldVertices(pbMesh.positions.Select((p, index) => index), 0.1f);
            var offset = i > _segments/2 ? new Vector3(0,-.5f/i,0) : new Vector3(0,-1f * (1+i)/_segments,0);
            debugPoints.Add(Random.ColorHSV(), new List<Vector3>());
            lastExtruded = ExtrudeFaces(pbMesh, offset, new Vector3(1f * (i+1)/_segments,0,1f * (i+1)/_segments), strength, lastExtruded);
        }
        
        pbMesh.GetComponent<Renderer>().sharedMaterial = grass;

        Noise(pbMesh);
        Smoothing.ApplySmoothingGroups(pbMesh, pbMesh.faces, 180);
        
        pbMesh.ToMesh();
        pbMesh.Refresh();
    }

    private void Noise(ProBuilderMesh pbMesh)
    {
        var verts = pbMesh.positions.Select((p, index) => new { Index = index, Position = p })
            .ToDictionary(p => p.Index, p => p.Position).GroupBy(v => (v.Value.x, v.Value.y, v.Value.z));
        
        foreach (var position in verts)
        {
            var randomOffset = new Vector3(
                Random.Range(-offsetRange, offsetRange),
                Random.Range(0, offsetRange),
                Random.Range(-offsetRange, offsetRange)
            );

            //var pos = position.ToList().First().Value + ;
            pbMesh.TranslateVertices(position.ToList().Select(v => v.Key).ToArray(), randomOffset);
        }
    }

    private Dictionary<Edge, Vector3> ExtrudeFaces(ProBuilderMesh pbMesh, Vector3 offset, Vector3 directionConstraint,
        float strength, Dictionary<Edge, Vector3> lastExtruded = null)
    {
        var edgesWithDirection = new Dictionary<Edge, Vector3>();
        if (lastExtruded == null)
        {
            var umschlosseneVerticies = new List<int>();
            foreach (var group in pbMesh.positions.Select((p, index) => new { Index = index, Position = p })
                         .ToDictionary(p => p.Index, p => p.Position).GroupBy(v => (v.Value.x, v.Value.y, v.Value.z)))
            {
                if (group.Count() >= 4)
                {
                    //Debug.Log(group.Key  + " Found Verticies:" + group.Count());
                    debugPoints.Last().Value.AddRange(group.Select(g => g.Value));
                    umschlosseneVerticies.AddRange(group.ToList().Select(v => v.Key));
                }
            }
            //pbMesh.Refresh();

            foreach (var face in pbMesh.faces.ToList())
            {
                var perEdges = pbMesh.GetPerimeterEdges(new List<Face> { face });
                var newEdges = pbMesh.Extrude(perEdges.Where(e => !umschlosseneVerticies.Any(u => e.Contains(u))), 0f,
                    false, true);
                
                if (newEdges != null)
                {
                    foreach (var edge in newEdges)
                    {
                        var direction = GetMiddleOfEdge(pbMesh, edge) - GetMiddleOfFace(pbMesh, face);
                        edgesWithDirection.Add(edge, direction);
                    }
                }
            }
        }
        else
        {
            foreach (var edgeWithDirection in lastExtruded)
            {
                var newEdges = pbMesh.Extrude(new[] { edgeWithDirection.Key }, 0f,
                    true, false);

                if (newEdges != null)
                {
                    foreach (var edge in newEdges)
                    {
                        edgesWithDirection.Add(edge, edgeWithDirection.Value);
                    }
                }
            }
        }
        
        foreach (var edge in edgesWithDirection)
        {
            var connectedVertices = edgesWithDirection.SelectMany(e =>
                new[] { e.Key.a, e.Key.b }.Where(v =>
                    pbMesh.positions[v] == pbMesh.positions[edge.Key.a] ||
                    pbMesh.positions[v] == pbMesh.positions[edge.Key.b])).ToList();
            
            var randomOffset = new Vector3(
                Random.Range(-offsetRange, offsetRange),
                Random.Range(-offsetRange, offsetRange),
                Random.Range(-offsetRange, offsetRange)
            );
            
            //pbMesh.TranslateVertices(connectedVertices, ElementWiseMultiply(directionConstraint, edge.Value).normalized * strength + offset);
            pbMesh.TranslateVertices(connectedVertices,
                (ElementWiseMultiply(directionConstraint, randomOffset + edge.Value).normalized) * (strength + Random.Range(0, offsetRange)));
        }

        foreach (var edge in edgesWithDirection)
        {
            pbMesh.TranslateVertices(new[] { edge.Key }, offset);
        }
        
        return edgesWithDirection;
    }
    
    public static Vector3 ElementWiseMultiply(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }
    
    Vector3 GetMiddleOfFace(ProBuilderMesh pbMesh, Face face)
    {
        // Get the vertices of the face
        var faceVertices = pbMesh.positions;
        var faceVertexPositions = face.edges.Select(e => faceVertices[e.a]).ToArray();

        // Calculate the average position of the face's vertices
        Vector3 sum = Vector3.zero;
        foreach (var vertex in faceVertexPositions)
        {
            sum += vertex;
        }
        
        return sum / faceVertexPositions.Length;
    }
    
    public void ApplySmoothShading(ProBuilderMesh pbMesh)
    {
        if (pbMesh == null)
        {
            Debug.LogError("ProBuilderMesh is null.");
            return;
        }

        // Calculate smooth normals
        pbMesh.ToMesh(); // Ensure mesh is up-to-date

        // Compute smooth normals
        Normals.CalculateNormals(pbMesh);

        // Refresh the ProBuilder mesh to apply the changes
        pbMesh.Refresh();
    }

    
    Vector3 GetMiddleOfEdge(ProBuilderMesh pbMesh, Edge edge)
    {
        // Get the positions of the vertices that define the edge
        var vertexPositions = pbMesh.positions;
        var vertexA = vertexPositions[edge.a];
        var vertexB = vertexPositions[edge.b];

        // Calculate the midpoint of the edge
        return (vertexA + vertexB) / 2f;
    }

    private void OnDrawGizmos()
    {
        foreach (var debug in debugPoints.TakeLast(1))
        {
            foreach (var debugPoint in debug.Value)
            {
                Gizmos.color = debug.Key;
                Gizmos.DrawSphere(debugPoint, 0.05f);
            }
        }
    }
}