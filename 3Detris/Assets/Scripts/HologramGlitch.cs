using System.Collections;
using UnityEngine;

public class HologramGlitch : MonoBehaviour
{
    public float glitchChance = 0.1f;

    Material hologramMaterial;
    readonly WaitForSeconds glitchLoopWait = new(0.1f);

    void Awake()
    {
        hologramMaterial = GetComponent<Renderer>().material;
    }
    IEnumerator Start()
    {
        while (true)
        {
            float glitchTest = Random.Range(0f, 1f);

            if (glitchTest <= glitchChance)
            {
                float originalGlowIntensity = hologramMaterial.GetFloat("_GlowIntensity");
                hologramMaterial.SetFloat("_GlitchIntensity", Random.Range(0.1f, 0.2f));
                hologramMaterial.SetFloat("_GlowIntensity", originalGlowIntensity * Random.Range(0.4f, 0.7f));
                yield return new WaitForSeconds(Random.Range(0.1f, 0.15f));
                hologramMaterial.SetFloat("_GlitchIntensity", 0f);
                hologramMaterial.SetFloat("_GlowIntensity", originalGlowIntensity);
            }

            yield return glitchLoopWait;
        }
    }
}