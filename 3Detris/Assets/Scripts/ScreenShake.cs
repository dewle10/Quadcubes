using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public float duration = 0.2f;
    public float magnitude = 0.1f;

    private Vector3 originalPos;
    private float elapsed = 0f;

    public void Shake()
    {
        originalPos = transform.localPosition;
        elapsed = duration;
    }

    void Update()
    {
        if (elapsed > 0)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPos + new Vector3(x, y, 0);

            elapsed -= Time.deltaTime;
            if (elapsed <= 0)
                transform.localPosition = originalPos;
        }
    }
}
