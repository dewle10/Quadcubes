using System.Collections;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ChangeColor : MonoBehaviour
{
    private MeshRenderer ren;
    private Color startColor;
    [SerializeField] private GameObject placeParticle;
    [SerializeField] private GameObject lineClearParticle;
    [SerializeField] private float colorChangeTime = 1f;
    [SerializeField] private float colorChangeRate = 1f;
    [SerializeField] private float colorChangeStart = 0.2f;

    void Start()
    {
        ren = GetComponent<MeshRenderer>();
        startColor = ren.material.color;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            PlayLCParticle();
        }
    }

    public IEnumerator Change(Color newColor)
    {
        float elapsed = 0f;
        while (elapsed < colorChangeTime)
        {
            elapsed += Time.deltaTime;
            Color lerpedColor = Color.Lerp(startColor, newColor, elapsed * colorChangeRate + colorChangeStart);
            ren.material.color = lerpedColor;
            yield return null;
        }
        ren.material.color = newColor;
    }
    public void PlayPlaceParticle()
    {
        GameObject particleObject = Instantiate(placeParticle, transform.position, Quaternion.identity);
        ParticleSystem particle = particleObject.GetComponent<ParticleSystem>();
        particle.Play();
    }
    public void PlayLCParticle()
    {
        MeshRenderer render = GetComponent<MeshRenderer>();
        Color startColorLC = render.material.color;
        Color newColor = startColorLC;
        newColor.a = 0f;
        GameObject particleObject = Instantiate(lineClearParticle, transform.position, Quaternion.identity);
        ChangeAlpha changeAlpha = particleObject.GetComponent<ChangeAlpha>();
        changeAlpha.StartCoroutine(changeAlpha.LerpAlpha(startColorLC, newColor));
    }
    
}
