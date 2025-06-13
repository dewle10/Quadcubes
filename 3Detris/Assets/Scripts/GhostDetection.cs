using UnityEngine;

public class GhostDetection : MonoBehaviour
{
    private LayerMask obstacleLayer;
    private Vector3 checkSize = new Vector3(0.9f, 0.9f, 0.9f);
    private Collider[] hits = new Collider[1];

    private void Start()
    {
        obstacleLayer = LayerMask.GetMask("Solid");
    }
    public bool GetghostHit(Vector3 directionVector)
    {
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

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + directionVector, checkSize);
    }*/
}
