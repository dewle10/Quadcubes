using UnityEngine;

public class WallKicks : MonoBehaviour
{
    [SerializeField] private Vector3 kickDirection;

    public Vector3 GetKickDirectionWall()
    {
        return kickDirection;
    }
}
