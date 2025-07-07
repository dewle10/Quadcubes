using UnityEngine;

public class CameraTracker : MonoBehaviour
{
    public Transform Center;
    public Transform cam;

    public enum Sector
    {
        Front = 0,
        Right = 1,
        Back = 2,
        Left = 3
    }
    public int GetCameraSector()
    {
        Vector3 dir = cam.position - Center.position;
        dir.y = 0;

        float angle = Vector3.SignedAngle(Center.forward, dir.normalized, Vector3.up);
        angle = (angle + 360f) % 360f; // Change from [-180, 180] to [0, 360]

        int index = Mathf.FloorToInt((angle + 45f) / 90f) % 4;
        return index;
    }

    private void Start()
    {
    }
}