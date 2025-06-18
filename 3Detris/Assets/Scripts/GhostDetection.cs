using UnityEngine;

public class GhostDetection : MonoBehaviour
{
    private LayerMask obstacleLayer;
    private Vector3 checkSize = new Vector3(0.9f, 0.9f, 0.9f);
    private Collider[] hits = new Collider[1];
    private Vector3 kickDirection;
    private bool wallHit = false;

    private void Start()
    {
        obstacleLayer = LayerMask.GetMask("Solid");
    }
    public bool GetghostHitMove(Vector3 directionVector)
    {
        hits[0] = null;
        int numHits = Physics.OverlapBoxNonAlloc(
            transform.position + directionVector,
            checkSize / 2,
            hits,
            transform.rotation,
            obstacleLayer
        );
        //if (numHits > 0)
            //Debug.Log(numHits+ " " + gameObject.name);
        return numHits > 0;
    }
    public bool GetghostHitRotate()
    {
        hits[0] = null;
        int numHits = Physics.OverlapBoxNonAlloc(
            transform.position,
            checkSize / 2,
            hits,
            transform.rotation,
            obstacleLayer
        );

        wallHit = false;
        kickDirection = Vector3.zero;
        if (hits[0] != null)
        {
            if (hits[0].gameObject.CompareTag("OutOfBounds"))
            {
                wallHit = true;
                WallKicks wallKick = hits[0].gameObject.GetComponent<WallKicks>();
                kickDirection = wallKick.GetKickDirectionWall();
                //Debug.Log(hits[0].gameObject);
            }
        }
        //if (numHits > 0)
        //  Debug.Log(numHits+ " " + gameObject.name);
        return numHits > 0;
    }

    public Vector3 GetKickDirection()
    {
        return kickDirection;
    }
    public bool GetWallHit()
    {
        return wallHit;
    }

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + directionVector, checkSize);
    }*/
}
