using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleScript : MonoBehaviour
{
    public int Subdivision = 0;

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();

    void Start()
    {
        Subdivision = Mathf.Clamp(Subdivision, 0, 3);
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        Subdivide(Subdivision);
        UpdateMesh(mesh);
    }

    private void OnValidate()
    {
        vertices.Clear();
        triangles.Clear();
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        Subdivide(Subdivision);
        UpdateMesh(mesh);
    }

    void CreateShape()
    {
        // Create multiple initial triangles
        // Triangle 1
        vertices.Add(new Vector3(-0.5f, -0.2887f, 0f)); // Bottom-left
        vertices.Add(new Vector3(0.5f, -0.2887f, 0f));  // Bottom-right
        vertices.Add(new Vector3(0f, 0.5774f, 0f));     // Top
        triangles.Add(0);
        triangles.Add(1);
        triangles.Add(2);

        // Triangle 2 (for example, another triangle next to the first one)
        vertices.Add(new Vector3(1.0f, -0.2887f, 0f)); // Bottom-left
        vertices.Add(new Vector3(2.0f, -0.2887f, 0f)); // Bottom-right
        vertices.Add(new Vector3(1.5f, 0.5774f, 0f));   // Top
        triangles.Add(3);
        triangles.Add(4);
        triangles.Add(5);

        // Add more triangles as needed...
    }

    void UpdateMesh(Mesh mesh)
    {
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
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
        Vector3 midpoint = (vertices[i1] + vertices[i2]) * 0.5f;

        if (vertices.Contains(midpoint))
        {
            return vertices.IndexOf(midpoint); // Avoid duplicate vertices
        }

        vertices.Add(midpoint);
        return vertices.Count - 1;
    }
}