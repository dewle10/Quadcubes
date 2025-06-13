using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float autoDestroytimerTime;
    private float autoDestroytimerCounter;

    private void Update()
    {
        autoDestroytimerCounter += Time.deltaTime;

        if( autoDestroytimerCounter >= autoDestroytimerTime)
        {
            Destroy(gameObject);
        }
    }
}
