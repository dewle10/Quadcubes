using UnityEngine;

public class GhostDetection : MonoBehaviour
{
    private LayerMask obstacleLayer;
    private Vector3 checkSize = new(0.9f, 0.9f, 0.9f);
    private Collider[] hits = new Collider[1];
    public Vector3 KickDirection { get; private set; }
    public bool WallHit { get; private set; }

    private void Start()
    {
        obstacleLayer = LayerMask.GetMask("Solid");
    }
    public bool GetghostHitMove(Vector3 directionVector) //Checks if moving would cause a collision
    {
        return CheckCollision(transform.position + directionVector) > 0;
    }
    public bool GetghostHitRotate() //Checks if rotation would cause a collision
    {
        int numHits = CheckCollision(transform.position);

        WallHit = false;
        KickDirection = Vector3.zero;
        if (hits[0] != null)
        {
            if (hits[0].gameObject.CompareTag("OutOfBounds"))
            {
                WallHit = true;
                WallKicks wallKick = hits[0].gameObject.GetComponent<WallKicks>();
                if (wallKick.Gethitted())
                    KickDirection = Vector3.zero;
                else
                    KickDirection = wallKick.GetKickDirectionWall();
            }
        }
        return numHits > 0;
    }

    private int CheckCollision(Vector3 position)
    {
        hits[0] = null;
        return Physics.OverlapBoxNonAlloc(
            position,
            checkSize / 2,
            hits,
            transform.rotation,
            obstacleLayer
        );
    }
}
