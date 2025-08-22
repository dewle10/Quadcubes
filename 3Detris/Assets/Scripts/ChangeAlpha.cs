using System.Collections;
using UnityEngine;

public class ChangeAlpha : MonoBehaviour
{
    [SerializeField] private float alphaChangeTime = 1f;
    [SerializeField] private float alphaChangeRate = 1f;
    [SerializeField] private float alphaChangeStart = 0.2f;

    public IEnumerator LerpAlpha(Color startColorLC, Color newColor)
    {
        ParticleSystem particle = GetComponent<ParticleSystem>();
        Material mat = GetComponent<ParticleSystemRenderer>().material;
        particle.Play();

        startColorLC.a = alphaChangeStart;

        float elapsed = 0f;
        while (elapsed < alphaChangeTime)
        {
            elapsed += Time.deltaTime;
            Color lerpedColor = Color.Lerp(startColorLC, newColor, elapsed * alphaChangeRate);
            mat.color = lerpedColor;
            //Debug.Log(elapsed);
            yield return null;
        }
        mat.color = newColor;
    }
}
