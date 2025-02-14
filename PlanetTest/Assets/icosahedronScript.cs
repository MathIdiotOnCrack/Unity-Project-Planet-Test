using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcosahedronScript : MonoBehaviour
{
    public int Subdivision = 0;

    private Mesh mesh;
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();

    float a = (float).525731112119133606;
    float b = (float).850650808352039932;

    void Start()
    {
        Subdivision = Mathf.Clamp(Subdivision, 0, 3);
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        Subdivide(Subdivision);
        UpdateMesh();
    }

    private void OnValidate()
    {
        vertices.Clear();
        triangles.Clear();
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        Subdivide(Subdivision);
        UpdateMesh();
    }

    void CreateShape()
    {
        vertices = new List<Vector3>
        {
            new Vector3(-a,  b,  0),
            new Vector3( a,  b,  0),
            new Vector3(-a, -b,  0),
            new Vector3( a, -b,  0),
            new Vector3( 0, -a,  b),
            new Vector3( 0,  a,  b),
            new Vector3( 0, -a, -b),
            new Vector3( 0,  a, -b),
            new Vector3( b,  0, -a),
            new Vector3( b,  0,  a),
            new Vector3(-b,  0, -a),
            new Vector3(-b,  0,  a)
        };

        // Project initial vertices onto sphere
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] = vertices[i].normalized;
        }

        triangles = new List<int>
        {
            0, 11, 5,
            0, 5, 1,
            0, 1, 7,
            0, 7, 10,
            0, 10, 11,
            1, 5, 9,
            5, 11, 4,
            11, 10, 2,
            10, 7, 6,
            7, 1, 8,
            3, 9, 4,
            3, 4, 2,
            3, 2, 6,
            3, 6, 8,
            3, 8, 9,
            4, 9, 5,
            2, 4, 11,
            6, 2, 10,
            8, 6, 7,
            9, 8, 1
        };
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    void Subdivide(int depth)
    {
        if (depth <= 0) return;

        List<int> newTriangles = new List<int>();

        for (int i = 0; i < triangles.Count; i += 3)
        {
            int a = triangles[i];
            int b = triangles[i + 1];
            int c = triangles[i + 2];

            // Get midpoints and add to vertices list
            int ab = AddMidpoint(a, b);
            int bc = AddMidpoint(b, c);
            int ca = AddMidpoint(c, a);

            // Create 4 new triangles
            newTriangles.AddRange(new int[] { a, ab, ca }); // Triangle 1
            newTriangles.AddRange(new int[] { ab, b, bc }); // Triangle 2
            newTriangles.AddRange(new int[] { ca, bc, c }); // Triangle 3
            newTriangles.AddRange(new int[] { ab, bc, ca }); // Center triangle
        }

        triangles = newTriangles; // Replace with new triangles
        Subdivide(depth - 1); // Recursively subdivide
    }

    int AddMidpoint(int i1, int i2)
    {
        Vector3 v1 = vertices[i1];
        Vector3 v2 = vertices[i2];
        Vector3 midpoint = (v1 + v2) * 0.5f;

        midpoint = midpoint.normalized; // Project midpoint onto sphere

        Vector3 scaledMidpoint = midpoint * Vector3.Distance(Vector3.zero, vertices[0]); // Scale midpoint to initial radius

        // Check for existing vertex
        int existingVertexIndex = -1;
        for (int i = 0; i < vertices.Count; i++)
        {
            if (Vector3.Distance(vertices[i], scaledMidpoint) < 0.0001f)
            {
                existingVertexIndex = i;
                break;
            }
        }

        if (existingVertexIndex != -1)
        {
            return existingVertexIndex; // Avoid duplicate vertices
        }

        vertices.Add(midpoint);
        return vertices.Count - 1;
    }

     
}