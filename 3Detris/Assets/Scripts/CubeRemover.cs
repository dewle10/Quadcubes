using UnityEngine;

public class CubeRemover : MonoBehaviour
{
    private int cubeCount;
    private void OnTriggerEnter(Collider other)
    {
        //if
        //cubeCount++;
        if(cubeCount == GridManager.gameWidth * GridManager.gameWidth)
        {
            Destroy(other.gameObject);
            Debug.Log("cubes removed");
        }
    }
}
