using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GridMesh3D : MonoBehaviour
{
    [Header("Grid Settings")]
    public Vector3Int gridSize = new Vector3Int(10, 5, 10); // Cells in X/Y/Z directions
    public float cellSize = 1f;
    public Color gridColor = Color.gray;

    [Header("References")]
    [SerializeField] private Material gridMaterial;

    private Mesh mesh;
    private List<Vector3> vertices;
    private List<int> indices;

    private void Start()
    {
        InitializeMesh();
        GenerateGrid();
        UpdateMesh();
    }

    private void InitializeMesh()
    {
        mesh = new Mesh();
        vertices = new List<Vector3>();
        indices = new List<int>();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = gridMaterial;
    }

    private void GenerateGrid()
    {
        // X Lines (Along Z Axis)
        for (int x = 0; x <= gridSize.x; x++)
        {
            for (int z = 0; z <= gridSize.z; z++)
            {
                Vector3 start = new Vector3(x * cellSize, 0, z * cellSize);
                Vector3 end = start + Vector3.up * (gridSize.y * cellSize);
                AddLine(start, end);
            }
        }

        // Y Lines (Along X Axis)
        for (int y = 0; y <= gridSize.y; y++)
        {
            for (int z = 0; z <= gridSize.z; z++)
            {
                Vector3 start = new Vector3(0, y * cellSize, z * cellSize);
                Vector3 end = start + Vector3.right * (gridSize.x * cellSize);
                AddLine(start, end);
            }
        }

        // Z Lines (Along X-Y Plane)
        for (int x = 0; x <= gridSize.x; x++)
        {
            for (int y = 0; y <= gridSize.y; y++)
            {
                Vector3 start = new Vector3(x * cellSize, y * cellSize, 0);
                Vector3 end = start + Vector3.forward * (gridSize.z * cellSize);
                AddLine(start, end);
            }
        }
    }

    private void AddLine(Vector3 start, Vector3 end)
    {
        vertices.Add(start);
        vertices.Add(end);
        indices.Add(vertices.Count - 2);
        indices.Add(vertices.Count - 1);
    }

    private void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
        mesh.RecalculateBounds();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (mesh != null && Application.isPlaying)
        {
            InitializeMesh();
            GenerateGrid();
            UpdateMesh();
        }
    }
#endif
}