using System.Collections;
using UnityEngine;

public class VisualEffects : MonoBehaviour
{
    [SerializeField] private GameObject placeParticle;
    [SerializeField] private GameObject placeParticleColor;
    [SerializeField] private GameObject lineClearParticle;
    [SerializeField] private float colorChangeTime = 1f;
    [SerializeField] private float colorChangeRate = 1f;
    [SerializeField] private float colorChangeStart = 0.2f; 

    private Color startColor;
    private LayerMask layerMask;
    private MeshRenderer ren;
    private GameObject hitObject;

    void Start()
    {
        layerMask = LayerMask.GetMask("Solid");
        ren = GetComponent<MeshRenderer>();
        startColor = ren.material.color;
    }

    public IEnumerator ChangeColor(Color newColor)
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
    public void PlayLCParticle() //Line Clear
    {
        MeshRenderer render = GetComponent<MeshRenderer>();
        Color startColorLC = render.material.color;
        Color newColor = startColorLC;
        newColor.a = 0f;
        GameObject particleObject = Instantiate(lineClearParticle, transform.position, Quaternion.identity);
        ChangeAlpha changeAlpha = particleObject.GetComponent<ChangeAlpha>();
        changeAlpha.StartCoroutine(changeAlpha.LerpAlpha(startColorLC, newColor)); //Fade effect
    }
    public void PlayPlaceParticle()
    {
        if (CheckBelow())
        {
            Vector3 pos = new(transform.position.x, transform.position.y - 0.5f, transform.position.z);

            MeshRenderer render = GetComponent<MeshRenderer>();
            SpawnPlaceParticleColor(render, pos, false);

            render = hitObject.GetComponentInChildren<MeshRenderer>();
            if (render.material.HasProperty("_Color")) 
                SpawnPlaceParticleColor(render, pos, true);
        }
        GameObject particleObjectWhite = Instantiate(placeParticle, transform.position, Quaternion.identity);
        ParticleSystem particleWhite = particleObjectWhite.GetComponent<ParticleSystem>();
        particleWhite.Play();
    }
    private void SpawnPlaceParticleColor(MeshRenderer renderer, Vector3 pos, bool isbelow)
    {
        Color newColor = renderer.material.color;
        float rottaionX = 0f;
        if (isbelow) rottaionX = 180f;

        GameObject particleObjectColor = Instantiate(placeParticleColor, pos, Quaternion.Euler(rottaionX, 0f, 0f));
        ParticleSystem particleColor = particleObjectColor.GetComponent<ParticleSystem>();
        ParticleSystemRenderer psr = particleColor.GetComponent<ParticleSystemRenderer>();
        MaterialPropertyBlock mpb = new();
        psr.GetPropertyBlock(mpb);
        mpb.SetColor("_Color", newColor);
        psr.SetPropertyBlock(mpb);
        particleColor.Play();
    }
    private bool CheckBelow()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f, layerMask))
        {
            hitObject = hit.collider.gameObject;
            return true;
        }
        return false;
    }
}
