using UnityEngine;

public class SceneShake : MonoBehaviour
{
    private static SceneShake instance;

    [SerializeField] private float duration;
    [SerializeField] private float magnitude;
    [SerializeField] private float strengthXZ;

    static private Vector3 originalPos;
    static private float timeLeft;

    private void Awake()
    {
        instance = this;
        originalPos = instance.transform.localPosition;
    }

    static public void Shake()
    {
        timeLeft = instance.duration;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Shake();
        }

        if (timeLeft > 0)
        {
            float strength = (timeLeft / duration) * magnitude;
            float x = Random.Range(-1f, 1f) * strengthXZ;
            float y = -strength;
            float z = Random.Range(-1f, 1f) * strengthXZ;

            transform.localPosition = originalPos + new Vector3(x, y, z);

            timeLeft -= Time.deltaTime;

            if (timeLeft <= 0)
                transform.localPosition = originalPos;
        }
    }
}
