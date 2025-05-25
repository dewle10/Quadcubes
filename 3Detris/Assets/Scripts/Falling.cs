using UnityEngine;

public class Falling : MonoBehaviour
{
    private bool falling = true;
    private bool aboveSolid;

    public float fallingSpeed = 2.0f;
    private float fallingTimerTime = 10.0f;
    private float fallingTimercounter;
    public GameObject[] cubes;

    LayerMask layerMask;
    private float rayDistance = 1.1f;

    public int fallenBlocks;

    private void Awake()
    {
        layerMask = LayerMask.GetMask("Solid");
    }
    private void FixedUpdate()
    {
        foreach (GameObject obj in cubes)
        {
            RaycastHit hit;
            if (Physics.Raycast(obj.transform.position, Vector3.down, out hit, rayDistance, layerMask))
            {
                Debug.DrawRay(obj.transform.position, Vector3.down * hit.distance, Color.yellow);
                //Debug.Log("Did Hit");
                aboveSolid = true;
                break;
            }
            else
            {
                Debug.DrawRay(obj.transform.position, Vector3.down * rayDistance, Color.white);
                aboveSolid = false;
                //Debug.Log("Did not Hit");
            }

        }
    }

    void Update()
    {
        if(falling)
        {
            fallingTimercounter += Time.deltaTime;

            if (fallingTimercounter >= fallingTimerTime / fallingSpeed)
            {
                if (aboveSolid)
                {
                    gameObject.layer = 6;
                    foreach (GameObject cube in cubes)
                        cube.layer = 6;

                    falling = false;
                    fallingTimercounter = 0;
                }
                else
                {
                    fallingTimercounter = 0;
                    transform.position += Vector3.down;
                    fallenBlocks++;
                }
            }
        }
    }
}
