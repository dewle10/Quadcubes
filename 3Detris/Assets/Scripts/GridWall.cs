using UnityEngine;

public class GridWall : MonoBehaviour
{
    public int width = 6;
    public int height = 12;
    public int depth = 6;
    public float cellSize = 1f;
    public float lineWidth = 0.05f;
    public Material lineMaterial;

    void Start()
    {
        CreateWall(new Vector3(0, 0, 0), Vector3.right, Vector3.up, width, height, "Front");

        CreateWall(new Vector3(0, 0, depth * cellSize), Vector3.right, Vector3.up, width, height, "Back");

        CreateWall(new Vector3(0, 0, 0), Vector3.forward, Vector3.up, depth, height, "Left");

        CreateWall(new Vector3(width * cellSize, 0, 0), Vector3.forward, Vector3.up, depth, height, "Right");
    }

    void CreateWall(Vector3 offset, Vector3 axisH, Vector3 axisV, int hCount, int vCount, string name)
    {
        GameObject wallParent = new GameObject(name + " Wall");
        wallParent.transform.SetParent(transform);

        for (int v = 0; v <= vCount; v++)
        {
            Vector3 start = offset + axisV * (v * cellSize);
            Vector3 end = start + axisH * (hCount * cellSize);
            CreateLine(start, end, wallParent.transform);
        }

        for (int h = 0; h <= hCount; h++)
        {
            Vector3 start = offset + axisH * (h * cellSize);
            Vector3 end = start + axisV * (vCount * cellSize);
            CreateLine(start, end, wallParent.transform);
        }
    }

    void CreateLine(Vector3 start, Vector3 end, Transform parent)
    {
        GameObject lineObj = new GameObject("GridLine");
        lineObj.transform.SetParent(parent);
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPositions(new Vector3[] { start, end });
        lr.material = lineMaterial;
        lr.widthMultiplier = lineWidth;
        lr.useWorldSpace = true;
        lr.numCapVertices = 2;
    }
}
