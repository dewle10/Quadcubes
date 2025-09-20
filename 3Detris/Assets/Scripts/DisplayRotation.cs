using UnityEngine;

public class DisplayRotation : MonoBehaviour
{
    public float rotationSpeed = 30f;

    private Vector3 pivot;

    void Start()
    {
        pivot = CalculateChildrenCenter();
    }

    void Update()
    {
        pivot = CalculateChildrenCenter();
        transform.RotateAround(pivot, transform.up, rotationSpeed * Time.deltaTime);
    }

    private Vector3 CalculateChildrenCenter()
    {
        int n = transform.childCount;
        if (n == 0) return transform.position;
        Vector3 sum = Vector3.zero;
        for (int i = 0; i < n; i++) sum += transform.GetChild(i).position;
        return sum / n;
    }
}