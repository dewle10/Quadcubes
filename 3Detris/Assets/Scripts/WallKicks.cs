using UnityEngine;

public class WallKicks : MonoBehaviour
{
    [SerializeField] private Vector3 kickDirection;
    private bool hitted;

    public Vector3 GetKickDirectionWall()
    {
        hitted = true;
        return kickDirection;
    }
    public bool Gethitted()
    {
        return hitted;
    }
    public void ResetHit()
    {
        hitted = false;
    }
}
